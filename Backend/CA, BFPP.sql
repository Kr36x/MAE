-- use AgrolinkDB

-- =====================================================================
--							CARGA ACADEMICA
-- spMAE_ListarCargaAcademica
-- para uar en el dgv
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_ListarCargaAcademica
    @Anio INT = NULL,
    @DocenteID INT = NULL,
	@GradoID INT = NULL,
    @SeccionID INT = NULL,
	@BusquedaDocente VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        ROW_NUMBER() OVER 
		(
            ORDER BY 
                C.Anio DESC,
                D.Nombre,
                G.NombreGrado,
                S.Letra,
                A.Nombre
        ) AS Num,

        C.CargaID,

        D.Nombre AS Docente,
        A.Nombre AS Asignatura,
        G.NombreGrado AS Grado,
        S.Letra AS Seccion,

        'Activo' AS Estado

    FROM CargaAcademica C

    INNER JOIN Docente D ON C.DocenteID = D.DocenteID
    INNER JOIN Asignatura A ON C.AsignaturaID = A.AsignaturaID
    INNER JOIN Seccion S ON C.SeccionID = S.SeccionID
    INNER JOIN Grado G ON S.GradoID = G.GradoID

    WHERE 
		C.Estado = 1
        AND (@Anio IS NULL OR C.Anio = @Anio)
        AND (@DocenteID IS NULL OR C.DocenteID = @DocenteID)
		AND (@GradoID IS NULL OR G.GradoID = @GradoID)
        AND (@SeccionID IS NULL OR C.SeccionID = @SeccionID)
		AND (@BusquedaDocente IS NULL OR D.Nombre LIKE '%' + REPLACE(@BusquedaDocente, ' ', '%') + '%')

    ORDER BY 
        C.Anio DESC,
        D.Nombre,
        G.NombreGrado,
        S.Letra,
        A.Nombre
END
GO

-- =====================================================================
--							CARGA ACADEMICA
-- spMAE_ListarCargaAcademicaxDocentexSecc
-- para llenar dgv detalle de carga academica por docente
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_ListarCargaAcademicaxDocentexSecc 
	@DocenteID INT, 
	@SeccionID INT
AS
BEGIN
	SELECT DISTINCT
        A.AsignaturaID, 
        A.Nombre AS Asignatura
    FROM CargaAcademica CA
    INNER JOIN Asignatura A ON CA.AsignaturaID = A.AsignaturaID
    WHERE 
        CA.DocenteID = @DocenteID 
        AND CA.SeccionID = @SeccionID
        AND CA.Estado = 1
    ORDER BY A.Nombre
END
GO

-- =====================================================================
--							CARGA ACADEMICA
-- spMAE_AgregarCargaAcademica
-- para btn reponsable de asignar nueva carga academica
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_AgregarCargaAcademica
    @DocenteID INT,
    @GradoID INT,
    @SeccionID INT,
    @AsignaturaID INT,
    @Anio INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        IF @DocenteID IS NULL OR @GradoID IS NULL 
           OR @SeccionID IS NULL OR @AsignaturaID IS NULL OR @Anio IS NULL
        BEGIN
            ;THROW 53000, 'ERROR: Todos los campos son obligatorios.', 1;
        END

        IF NOT EXISTS 
		(
            SELECT 1 
            FROM Seccion 
            WHERE SeccionID = @SeccionID 
              AND GradoID = @GradoID
        )
        BEGIN
            ;THROW 53001, 'ERROR: La sección no pertenece al grado seleccionado.', 1;
        END

        IF EXISTS (
            SELECT 1
            FROM CargaAcademica
            WHERE DocenteID = @DocenteID
              AND AsignaturaID = @AsignaturaID
              AND SeccionID = @SeccionID
              AND Anio = @Anio
              AND Estado = 1
        )
        BEGIN
            ;THROW 53002, 'ERROR: Esta asignación ya existe y está activa.', 1;
        END

        IF EXISTS (
            SELECT 1
            FROM CargaAcademica
            WHERE AsignaturaID = @AsignaturaID
              AND SeccionID = @SeccionID
              AND Anio = @Anio
              AND Estado = 1
        )
        BEGIN
            ;THROW 53003, 'ERROR: Esta asignatura ya está asignada a otro docente en esta sección.', 1;
        END

        INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado, Anio)
        VALUES (@DocenteID, @AsignaturaID, @SeccionID, 1, @Anio);

        DECLARE @NuevoID INT = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT @NuevoID AS CargaID;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO

-- =====================================================================
--							CARGA ACADEMICA
-- spMAE_EditarCargaAcademica
-- solo permite editar docente
-- cambiar asignatura
-- no elimina solo activa o inactiva
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_EditarCargaAcademica
    @CargaID INT,
    @DocenteID INT,
    @AsignaturaID INT,
    @Estado BIT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1 FROM CargaAcademica WHERE CargaID = @CargaID
        )
        BEGIN
            ;THROW 55000, 'ERROR: La carga académica no existe.', 1;
        END

        IF EXISTS 
		(
            SELECT 1 FROM Configuracion
            WHERE Activa = 0 -- cerrado
        )
        BEGIN
            ;THROW 55001, 'ERROR: El periodo académico está cerrado. No se permiten cambios.', 1;
        END

        IF EXISTS (
            SELECT 1 FROM Actividad
            WHERE CargaID = @CargaID
        )
        BEGIN
            ;THROW 55002, 'ERROR: No se puede modificar. Ya existen actividades registradas.', 1;
        END

        IF EXISTS 
		(
            SELECT 1 FROM CargaAcademica
            WHERE DocenteID = @DocenteID
              AND AsignaturaID = @AsignaturaID
              AND SeccionID = (SELECT SeccionID FROM CargaAcademica WHERE CargaID = @CargaID)
              AND Anio = (SELECT Anio FROM CargaAcademica WHERE CargaID = @CargaID)
              AND Estado = 1
              AND CargaID <> @CargaID
        )
        BEGIN
            ;THROW 55003, 'ERROR: Ya existe una carga académica igual activa.', 1;
        END

        UPDATE CargaAcademica
        SET 
            DocenteID = @DocenteID,
            AsignaturaID = @AsignaturaID,
            Estado = @Estado
        WHERE CargaID = @CargaID;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO


CREATE OR ALTER PROCEDURE spMAE_InactivarCargaAcademica
    @CargaID INT
AS
BEGIN

    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM CargaAcademica WHERE CargaID = @CargaID)
        BEGIN
            ;THROW 56000, 'ERROR: La carga no existe.', 1;
        END

        IF EXISTS (SELECT 1 FROM Configuracion WHERE Activa = 0)
        BEGIN
            ;THROW 56001, 'ERROR: El período está cerrado.', 1;
        END

        IF EXISTS (SELECT 1 FROM Actividad WHERE CargaID = @CargaID)
        BEGIN
            ;THROW 56002, 'ERROR: No se puede eliminar, tiene actividades registradas.', 1;
        END

        UPDATE CargaAcademica
        SET Estado = 0
        WHERE CargaID = @CargaID;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO

-- =====================================================================
--							BOLETA FINAL
-- spMAE_BoletaxParcial
-- 
-- =====================================================================

CREATE OR ALTER PROCEDURE spMAE_BoletaxParcial
    @GradoID INT = NULL,
    @SeccionID INT = NULL,
    @Parcial INT, -- 1,2,3,4
    @Anio INT,
    @NombreEstudiante VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER (ORDER BY E.Nombre) AS Numero,
        E.Nombre AS Estudiante,
        E.Identidad,

        AVG
		(
		CASE 
                WHEN @Parcial = 1 THEN BD.NotaP1
                WHEN @Parcial = 2 THEN BD.NotaP2
                WHEN @Parcial = 3 THEN BD.NotaP3
                WHEN @Parcial = 4 THEN BD.NotaP4
            END
        ) AS Promedio,

        CASE 
            WHEN AVG
			(
                CASE 
                    WHEN @Parcial = 1 THEN BD.NotaP1
                    WHEN @Parcial = 2 THEN BD.NotaP2
                    WHEN @Parcial = 3 THEN BD.NotaP3
                    WHEN @Parcial = 4 THEN BD.NotaP4
                END
            ) >= 60 THEN 'APROBADO'
            ELSE 'REPROBADO'
        END AS Estado

    FROM Boleta B
    INNER JOIN Estudiante E ON B.EstudianteID = E.EstudianteID
    INNER JOIN BoletaDetalle BD ON B.BoletaID = BD.BoletaID
    INNER JOIN CargaAcademica CA ON B.DocenteID = CA.DocenteID

    WHERE 
        B.Anio = @Anio
        AND (@GradoID IS NULL OR CA.SeccionID IN (
            SELECT SeccionID FROM Seccion WHERE GradoID = @GradoID
        ))
        AND (@SeccionID IS NULL OR CA.SeccionID = @SeccionID)
        AND (@NombreEstudiante IS NULL OR E.Nombre LIKE '%' + @NombreEstudiante + '%')

    GROUP BY 
        E.Nombre,
        E.Identidad

    ORDER BY E.Nombre;

END
GO


CREATE OR ALTER PROCEDURE spMAE_ListarCicloEscolarActivo
AS
BEGIN
    SELECT DISTINCT Anio
    FROM Configuracion
    WHERE Activa = 1
    ORDER BY Anio DESC;
END
GO


CREATE OR ALTER PROCEDURE spMAE_BuscarBoletaxEstudiante
    @Nombre VARCHAR(100)
AS
BEGIN
    SELECT 
        EstudianteID,
        Nombre,
        Identidad
    FROM Estudiante
    WHERE Nombre LIKE '%' + @Nombre + '%'
    ORDER BY Nombre;
END
GO