-- use AgroLinkDB


-- =====================================================================
--								ACTIVIDAD
-- SP: spMAE_CrearActividad
-- Inserta una actividad y validando que el acumulado no exceda el 100%
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_CrearActividad
    @CargaID INT,
    @Parcial INT,
    @Descripcion VARCHAR(100),
    @Valor DECIMAL(5,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

		-- 0. valida que la descripción no sea nula, vacía o solo espacios (anti dummies)
		IF LTRIM(RTRIM(ISNULL(@Descripcion, ''))) = ''
		BEGIN
			;THROW 52000, 'ERROR: La descripción de la actividad es obligatoria y no puede estar vacía.', 1;
		END

		BEGIN TRANSACTION;

        -- 1. valida el rango del valor
        IF @Valor <= 0 OR @Valor > 100
        BEGIN
            ;THROW 52001, 'ERROR: El valor de la actividad debe ser mayor a 0 y máximo 100.', 1;
        END

        -- 2. calcula suma actual con bloqueo para evitar mistakes
        DECLARE @SumaActual DECIMAL(5,2);

        SELECT @SumaActual = ISNULL(SUM(Valor), 0)
        FROM Actividad WITH (UPDLOCK, HOLDLOCK)
        WHERE CargaID = @CargaID 
          AND Parcial = @Parcial;

		-- 3. obtiene fecha fin del parcial
		DECLARE @ParcialActivo INT, @FechaFin DATE;
		
		SELECT 
			@ParcialActivo = Periodo,
			@FechaFin = FechaFin
		FROM Configuracion
		WHERE Activa = 1;

		IF @Parcial <> @ParcialActivo
			THROW 50070, 'ERROR: Solo se pueden crear actividades antes del cierre del periodo.', 1;

		IF GETDATE() > @FechaFin
			THROW 50071, 'ERROR: El parcial activo ya cerró, no se pueden crear actividades.', 1;

        -- 4. valida que se limite hasta 100%
        IF (@SumaActual + @Valor) > 100
        BEGIN
            DECLARE @Faltante DECIMAL(5,2) = 100 - @SumaActual;
            -- se convierte de decimal a texto para la concatenación
            DECLARE @MensajeError VARCHAR(200) = 'ERROR: No se puede crear. Solo dispone de ' + FORMAT(@Faltante, 'N2') + '% libre en este parcial.';
            
            ;THROW 52008, @MensajeError, 1;
        END

        -- 4. se inserta actividad
        INSERT INTO Actividad (CargaID, Parcial, Descripcion, Valor)
        VALUES (@CargaID, @Parcial, UPPER(@Descripcion), @Valor);

        -- 5. obtenemos el id de la ultima actividad
        DECLARE @NuevoID INT = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        -- rertorno de ID final
        SELECT @NuevoID AS ActividadInsertadaID;

    END TRY
    BEGIN CATCH
        -- si hay una transacción abierta, deshacer los cambios
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- re-lanzar el error para que frontend lo capture
        ;THROW;
    END CATCH
END
GO

-- =====================================================================
--						  ACTIVIDAD (SUB)
-- SP: spMAE_ListarActividadesPorParcial
-- Filtro de sp por parcial
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_ListarActividadesPorParcial
    @DocenteID INT,
    @CargaID INT,
    @Parcial INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER (ORDER BY A.ActividadID) AS Num,
        A.ActividadID,
        A.Descripcion,
        A.Valor
    FROM Actividad A
    INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
    WHERE C.DocenteID = @DocenteID
      AND A.CargaID = @CargaID
      AND (@Parcial IS NULL OR A.Parcial = @Parcial)
END
GO

-- =====================================================================
--						  ACTIVIDAD (SUB)
-- SP: spMAE_BuscarActividades
-- Buscar por actividad/decripcion de la mima
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_BuscarActividades
    @DocenteID INT,
    @TextoBusqueda VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT A.*
    FROM Actividad A
    INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
    WHERE C.DocenteID = @DocenteID
      AND UPPER(A.Descripcion) LIKE UPPER('%' + @TextoBusqueda + '%')
END
GO

-- =====================================================================
--						 ACTIVIDAD (SUB)
-- SP: spMAE_EditarActividad
-- Editar actividad + validación de docente login
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_EditarActividad
    @ActividadID INT,
    @Descripcion VARCHAR(100),
    @Valor DECIMAL(5,2),
    @DocenteID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- valida que le pertenece al docente logueado
    IF NOT EXISTS 
	(
        SELECT 1
        FROM Actividad A
        INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
        WHERE A.ActividadID = @ActividadID
          AND C.DocenteID = @DocenteID
    )
        THROW 53011, 'ERROR: No tiene permisos para editar esta actividad.', 1;
	
		DECLARE @FechaFin DATE;
			SELECT @FechaFin = P.FechaFin
			FROM Actividad A
			INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
			INNER JOIN Configuracion P ON P.Activa = 1
			WHERE A.ActividadID = @ActividadID;

		IF GETDATE() > @FechaFin
			THROW 50061, 'ERROR: Fuera del periodo permitido. Solo se pueden editar actividades dentro del rango configurado.', 1;

    UPDATE Actividad
    SET Descripcion = UPPER(@Descripcion),
        Valor = @Valor
    WHERE ActividadID = @ActividadID;
END
GO

-- =====================================================================
--						ACTIVIDAD (SUB)
-- SP: spMAE_EliminarActividad
-- Eliminar actividad + validación de docente login
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_EliminarActividad
    @ActividadID INT,
    @DocenteID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. valida que la actividad pertenece al docente
        IF NOT EXISTS 
		(
            SELECT 1
            FROM Actividad A
            INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
            WHERE A.ActividadID = @ActividadID
              AND C.DocenteID = @DocenteID
        )
            THROW 50040, 'ERROR: No tiene permisos para eliminar esta actividad.', 1;

        -- 2. valida si tiene calificaciones (JOIN)
        IF EXISTS 
		(
            SELECT 1
            FROM Actividad A
            INNER JOIN Calificacion CA ON A.ActividadID = CA.ActividadID
            WHERE A.ActividadID = @ActividadID
        )
        BEGIN
            ;THROW 52002, 'ERROR: No se puede eliminar la actividad porque ya cuenta con calificaciones registradas.', 1;
        END

        -- 3. elimina
        DELETE FROM Actividad
        WHERE ActividadID = @ActividadID;

        COMMIT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END;
GO




-- =====================================================================
--							CALIFICACIÓN
-- SP: spMAE_GuardarCalificacion
-- Registra o actualiza la nota de un estudiante UPDATE E INSERT
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_GuardarCalificacion
    @EstudianteID INT,
    @ActividadID INT,
    @Nota DECIMAL(5,2),
    @Fecha DATE,
    @DocenteID INT 
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- =========================================================================
        -- 0. OBTENCIÓN DE CONFIGURACIÓN Y PARÁMETROS DE CONTROL
        -- =========================================================================
        
		DECLARE @ParcialActivo INT, @FechaInicio DATE, @FechaFin DATE;

        SELECT 
            @ParcialActivo = Periodo,
            @FechaInicio = FechaInicio,
            @FechaFin = FechaFin
        FROM Configuracion
        WHERE Activa = 1;

        -- =========================================================================
        -- 1. VALIDACIONES DE SEGURIDAD CRONOLÓGICA
        -- =========================================================================
        
        -- A. no permitir fechas futuras
        IF @Fecha > CAST(GETDATE() AS DATE)
            THROW 54012, 'ERROR: No se pueden registrar calificaciones con fecha futura.', 1;

        -- B. La fecha debe estar dentro del rango del parcial configurado
        IF @Fecha < @FechaInicio OR @Fecha > @FechaFin
            THROW 54013, 'ERROR: La fecha de la nota está fuera del rango del parcial activo.', 1;

        -- C. Validar que el parcial no haya cerrado administrativamente
        IF GETDATE() > @FechaFin
            THROW 54011, 'ERROR: El periodo de ingreso de notas ha finalizado según la configuración.', 1;

        -- =========================================================================
        -- 2. VALIDACIONES DE INTEGRIDAD DE ACTIVIDAD
        -- =========================================================================
        
        DECLARE @ActividadParcial INT, @ValorMaximo DECIMAL(5,2);

        SELECT 
            @ActividadParcial = Parcial, 
            @ValorMaximo = Valor 
        FROM Actividad 
        WHERE ActividadID = @ActividadID;

        -- A. validar existencia
        IF @ValorMaximo IS NULL
            THROW 53010, 'ERROR: La actividad especificada no existe en el catálogo.', 1;

        -- B. validar que la actividad corresponda al parcial que se está evaluando
        IF @ActividadParcial <> @ParcialActivo
            THROW 54010, 'ERROR: No se pueden asignar notas a actividades de parciales anteriores o futuros.', 1;

        -- =========================================================================
        -- 3. VALIDACIÓN DE PERMISOS Y RANGO DE NOTA
        -- =========================================================================

        -- A. validar que la actividad pertenezca a la carga académica del docente logueado
        IF NOT EXISTS (
            SELECT 1 
            FROM Actividad A 
            INNER JOIN CargaAcademica C ON A.CargaID = C.CargaID
            WHERE A.ActividadID = @ActividadID 
              AND C.DocenteID = @DocenteID
        )
        BEGIN
            ;THROW 53011, 'ERROR: Acceso denegado. Usted no es el docente titular de esta asignatura.', 1;
        END

        -- B. validar que la nota no exceda el puntaje máximo (70%, 30%, etc.)
        IF @Nota < 0 OR @Nota > @ValorMaximo
        BEGIN
            DECLARE @MsgNota VARCHAR(200) = 
                'La nota ingresada (' + CAST(@Nota AS VARCHAR(10)) + 
                ') supera el valor máximo permitido (' + CAST(@ValorMaximo AS VARCHAR(10)) + ') para esta actividad.';
            THROW 53012, @MsgNota, 1;
        END

        -- =========================================================================
        -- 4. PERSISTENCIA DE DATOS (UPSERT LÓGICO)
        -- =========================================================================
        BEGIN TRANSACTION;

        -- Usamos bloqueos de fila para evitar colisiones de datos
        IF EXISTS (
            SELECT 1 
            FROM Calificacion WITH (UPDLOCK, HOLDLOCK) 
            WHERE EstudianteID = @EstudianteID 
              AND ActividadID = @ActividadID
        )
        BEGIN
            -- actualización de nota existente
            UPDATE Calificacion
            SET Nota = @Nota,
                Fecha = @Fecha
            WHERE EstudianteID = @EstudianteID 
              AND ActividadID = @ActividadID;
        END
        ELSE
        BEGIN
            -- inserción de nueva nota
            INSERT INTO Calificacion (EstudianteID, ActividadID, Nota, Fecha)
            VALUES (@EstudianteID, @ActividadID, @Nota, @Fecha);
        END

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        -- manejo de reversión en caso de falla
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- relanzar el error
        THROW;
    END CATCH
END
GO

-- =====================================================================
--							CALIFICACIÓN
-- SP: spMAE_ListarCalificaciones
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_ListarCalificaciones
    @DocenteID INT,
    @Anio INT = NULL,
    @GradoID INT = NULL,
    @SeccionID INT = NULL,
    @ActividadID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER (ORDER BY E.Nombre) AS Num,
        E.EstudianteID,
        E.Nombre AS Estudiante,
        A.Descripcion AS Actividad,
        C.Nota,
        C.Fecha,
        C.CalificacionID
    FROM Calificacion C
    INNER JOIN Estudiante E ON C.EstudianteID = E.EstudianteID
    INNER JOIN Actividad A ON C.ActividadID = A.ActividadID
    INNER JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
	INNER JOIN Seccion S ON S.SeccionID = CA.SeccionID
    WHERE CA.DocenteID = @DocenteID
      AND (@Anio IS NULL OR CA.Anio = @Anio)
      AND (@GradoID IS NULL OR S.GradoID = @GradoID)
      AND (@SeccionID IS NULL OR CA.SeccionID = @SeccionID)
      AND (@ActividadID IS NULL OR A.ActividadID = @ActividadID)
END
GO

-- =====================================================================
--							CALIFICACIÓN
-- SP: spMAE_BuscarCalificacionesPorEstudiante
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_BuscarCalificacionesPorEstudiante
    @DocenteID INT,
    @NombreEstudiante VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER (ORDER BY E.Nombre) AS Num,
        E.EstudianteID,
        E.Nombre AS Estudiante,
        A.Descripcion AS Actividad,
        C.Nota,
        C.Fecha,
        C.CalificacionID
    FROM Calificacion C
    INNER JOIN Estudiante E ON C.EstudianteID = E.EstudianteID
    INNER JOIN Actividad A ON C.ActividadID = A.ActividadID
    INNER JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
    WHERE CA.DocenteID = @DocenteID
      AND (E.Nombre LIKE '%' + @NombreEstudiante + '%')
END
GO

-- =====================================================================
--							CALIFICACIÓN
-- SP: spMAE_GuardarCalificaciones
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_GuardarCalificacionesMasivo
    @CalificacionID INT,
    @Nota DECIMAL(5,2),
    @Fecha DATE,
    @DocenteID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- valida que la calificación existe y pertenece al docente
        IF NOT EXISTS (
            SELECT 1
            FROM Calificacion C
            INNER JOIN Actividad A ON C.ActividadID = A.ActividadID
            INNER JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
            WHERE C.CalificacionID = @CalificacionID
              AND CA.DocenteID = @DocenteID
        )
            THROW 55010, 'ERROR: No tiene permisos para modificar esta calificación.', 1;

        -- valida valor máximo de la actividad
        DECLARE @ValorMaximo DECIMAL(5,2);
        SELECT @ValorMaximo = Valor FROM Actividad A
        INNER JOIN Calificacion C ON A.ActividadID = C.ActividadID
        WHERE C.CalificacionID = @CalificacionID;

        IF @Nota < 0 OR @Nota > @ValorMaximo
            THROW 55011, 'ERROR: La nota no puede superar el valor máximo de la actividad.', 1;

        -- valida fecha del parcial
        DECLARE @Parcial INT, @FechaFin DATE;
        SELECT @Parcial = A.Parcial, @FechaFin = P.FechaFin
        FROM Calificacion C
        INNER JOIN Actividad A ON C.ActividadID = A.ActividadID
        INNER JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
        INNER JOIN Configuracion P ON P.Activa = 1
        WHERE C.CalificacionID = @CalificacionID;

        IF @Parcial <> (SELECT Periodo FROM Configuracion WHERE Activa = 1)
            THROW 55012, 'ERROR: Solo se pueden modificar notas del parcial activo.', 1;

        IF GETDATE() > @FechaFin
            THROW 55013, 'ERROR: El parcial activo ya cerró.', 1;

        -- Actualizar nota
        UPDATE Calificacion
        SET Nota = @Nota,
            Fecha = @Fecha
        WHERE CalificacionID = @CalificacionID;

        COMMIT TRANSACTION;

        -- Retornar ID actualizado
        SELECT @CalificacionID AS CalificacionActualizadaID;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO

-- =====================================================================
--				    CONFIGURACION DE CICLO ESCOLAR
-- SP: spMAE_CrearConfiguracion
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_CrearConfiguracion
    @Periodo INT,
    @FechaInicio DATE,
    @FechaFin DATE,
    @CicloEscolar VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. validar que fecha fin no sea menor a inicio
        IF @FechaFin < @FechaInicio
            THROW 52001, 'ERROR: La fecha de finalización no puede ser menor a la fecha de inicio.', 1;

        -- 2. obtener último periodo registrado
        DECLARE @UltimaFechaFin DATE;

        SELECT TOP 1 @UltimaFechaFin = FechaFin
        FROM Configuracion
        WHERE CicloEscolar = @CicloEscolar
        ORDER BY Periodo DESC;

        -- 3. validar continuidad
        IF @UltimaFechaFin IS NOT NULL AND @FechaInicio <= @UltimaFechaFin
            THROW 52002, 'ERROR: La fecha de inicio debe ser mayor al cierre del periodo anterior.', 1;

        -- 4. obtener año desde fecha inicio
        DECLARE @Anio INT = YEAR(@FechaInicio);

        -- 5. insertar
        INSERT INTO Configuracion (Anio, Periodo, Activa, FechaInicio, FechaFin, CicloEscolar)
        VALUES (@Anio, @Periodo, 0, @FechaInicio, @FechaFin, @CicloEscolar);

        COMMIT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- =====================================================================
--				    CONFIGURACION DE CICLO ESCOLAR
-- SP: spMAE_EditarConfiguracion
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_EditarConfiguracion
    @ConfigID INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @FechaInicioActual DATE;

        SELECT @FechaInicioActual = FechaInicio
        FROM Configuracion
        WHERE ConfigID = @ConfigID;

        -- 1. validar fecha fin
        IF @FechaFin < @FechaInicio
            THROW 60010, 'ERROR: La fecha final no puede ser menor a la inicial.', 1;

        -- 2. si ya empezó → NO permitir cambiar fecha inicio
        IF GETDATE() >= @FechaInicioActual
        BEGIN
            -- solo actualizar fecha fin
            UPDATE Configuracion
            SET FechaFin = @FechaFin
            WHERE ConfigID = @ConfigID;
        END
        ELSE
        BEGIN
            -- validar contra periodo anterior
            DECLARE @FechaFinAnterior DATE;

            SELECT TOP 1 @FechaFinAnterior = FechaFin
            FROM Configuracion
            WHERE ConfigID < @ConfigID
            ORDER BY ConfigID DESC;

            IF @FechaFinAnterior IS NOT NULL AND @FechaInicio <= @FechaFinAnterior
                THROW 60011, 'ERROR: La nueva fecha de inicio invade el periodo anterior.', 1;

            UPDATE Configuracion
            SET FechaInicio = @FechaInicio,
                FechaFin = @FechaFin,
                Anio = YEAR(@FechaInicio)
            WHERE ConfigID = @ConfigID;
        END

        COMMIT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- =====================================================================
--				    CONFIGURACION DE CICLO ESCOLAR
-- SP: spMAE_BuscarConfiguracionPorCiclo
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_BuscarConfiguracionPorCiclo
    @CicloEscolar VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ConfigID,
        Anio,
        Periodo,
        FechaInicio,
        FechaFin,
        CicloEscolar,
        CASE 
            WHEN GETDATE() BETWEEN FechaInicio AND FechaFin THEN 'ACTIVO'
            WHEN GETDATE() < FechaInicio THEN 'PRÓXIMO'
            ELSE 'CERRADO'
        END AS Estado
    FROM Configuracion
    WHERE CicloEscolar = @CicloEscolar
    ORDER BY Periodo;
END