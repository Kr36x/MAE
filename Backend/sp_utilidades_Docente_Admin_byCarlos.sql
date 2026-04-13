USE AgroLinkDB;
GO

/* =========================================================
   ASIGNATURAS
   ========================================================= */

/*
    Lista las áreas curriculares únicas registradas en la tabla Asignatura.
    Se usa normalmente para llenar combos o filtros por área.
*/
CREATE OR ALTER PROCEDURE spMAE_ListarAreasAsignatura
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        LTRIM(RTRIM(Area)) AS Area
    FROM Asignatura
    WHERE ISNULL(LTRIM(RTRIM(Area)), '') <> ''
    ORDER BY LTRIM(RTRIM(Area));
END;
GO

/*
    Lista las asignaturas permitiendo filtrar por área
    y realizar una búsqueda por nombre, área o descripción.
*/
CREATE OR ALTER PROCEDURE spMAE_ListarAsignaturas
    @Area     VARCHAR(100) = '',
    @Busqueda VARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SET @Area = LTRIM(RTRIM(ISNULL(@Area, '')));
    SET @Busqueda = LTRIM(RTRIM(ISNULL(@Busqueda, '')));

    SELECT
        A.AsignaturaID,
        A.Nombre,
        A.Area,
        A.Descripcion
    FROM Asignatura A
    WHERE
        (
            @Area = ''
            OR LTRIM(RTRIM(A.Area)) = @Area
        )
        AND
        (
            @Busqueda = ''
            OR LTRIM(RTRIM(A.Nombre)) LIKE '%' + @Busqueda + '%'
            OR LTRIM(RTRIM(A.Area)) LIKE '%' + @Busqueda + '%'
            OR LTRIM(RTRIM(A.Descripcion)) LIKE '%' + @Busqueda + '%'
        )
    ORDER BY
        A.Area ASC,
        A.Nombre ASC;
END;
GO

/*
    Obtiene una asignatura específica por su ID.
    Útil para edición o visualización de detalle.
*/
CREATE OR ALTER PROCEDURE spMAE_ObtenerAsignaturaPorId
    @AsignaturaID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        A.AsignaturaID,
        A.Nombre,
        A.Area,
        A.Descripcion
    FROM Asignatura A
    WHERE A.AsignaturaID = @AsignaturaID;
END;
GO

/*
    Crea una nueva asignatura.
    Valida que nombre y área sean obligatorios
    y que no exista otra asignatura con el mismo nombre.
*/
CREATE OR ALTER PROCEDURE spMAE_CrearAsignatura
    @Nombre      VARCHAR(60),
    @Area        VARCHAR(100),
    @Descripcion VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        SET @Nombre = UPPER(LTRIM(RTRIM(@Nombre)));
        SET @Area = UPPER(LTRIM(RTRIM(@Area)));
        SET @Descripcion = LTRIM(RTRIM(ISNULL(@Descripcion, '')));

        IF @Nombre = ''
            THROW 50001, 'ERROR: El nombre de la asignatura es obligatorio.', 1;

        IF @Area = ''
            THROW 50002, 'ERROR: El área curricular es obligatoria.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM Asignatura
            WHERE UPPER(LTRIM(RTRIM(Nombre))) = @Nombre
        )
        BEGIN
            THROW 50006, 'ERROR: Ya existe una asignatura con ese nombre.', 1;
        END

        INSERT INTO Asignatura (Nombre, Area, Descripcion)
        VALUES (@Nombre, @Area, @Descripcion);

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END;
GO

/*
    Edita una asignatura existente.
    Verifica que exista, valida campos obligatorios
    y evita duplicar el nombre con otra asignatura.
*/
CREATE OR ALTER PROCEDURE spMAE_EditarAsignatura
    @AsignaturaID INT,
    @Nombre       VARCHAR(60),
    @Area         VARCHAR(100),
    @Descripcion  VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        SET @Nombre = UPPER(LTRIM(RTRIM(@Nombre)));
        SET @Area = UPPER(LTRIM(RTRIM(@Area)));
        SET @Descripcion = LTRIM(RTRIM(ISNULL(@Descripcion, '')));

        IF NOT EXISTS (SELECT 1 FROM Asignatura WHERE AsignaturaID = @AsignaturaID)
            THROW 50003, 'ERROR: La asignatura no existe.', 1;

        IF @Nombre = ''
            THROW 50001, 'ERROR: El nombre de la asignatura es obligatorio.', 1;

        IF @Area = ''
            THROW 50002, 'ERROR: El área curricular es obligatoria.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM Asignatura
            WHERE UPPER(LTRIM(RTRIM(Nombre))) = @Nombre
              AND AsignaturaID <> @AsignaturaID
        )
        BEGIN
            THROW 50007, 'ERROR: Ya existe otra asignatura con ese nombre.', 1;
        END

        UPDATE Asignatura
           SET Nombre = @Nombre,
               Area = @Area,
               Descripcion = @Descripcion
         WHERE AsignaturaID = @AsignaturaID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END;
GO

/*
    Elimina una asignatura por su ID.
    Primero valida que la asignatura exista.
*/
CREATE OR ALTER PROCEDURE spMAE_EliminarAsignatura
    @AsignaturaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Asignatura WHERE AsignaturaID = @AsignaturaID)
            THROW 50003, 'ERROR: La asignatura no existe.', 1;

        DELETE FROM Asignatura
        WHERE AsignaturaID = @AsignaturaID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END;
GO


/* =========================================================
   TUTORES
   ========================================================= */

/*
    Lista los tutores registrados.
    Permite filtrar por parentesco y buscar por nombre o identidad.
    También devuelve el estado del usuario vinculado.
*/
CREATE OR ALTER PROCEDURE sp_MAE_ListarTutores
    @Parentesco VARCHAR(50) = '',
    @Busqueda   VARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        T.TutorID,
        T.UsuarioID,
        T.Nombre,
        T.Identidad,
        T.Telefono,
        T.Parentesco,
        T.LugarTrabajo,
        U.Estado
    FROM Tutor T
    INNER JOIN Usuario U
        ON U.UsuarioID = T.UsuarioID
    WHERE
        (@Parentesco = '' OR T.Parentesco = @Parentesco)
        AND
        (
            @Busqueda = ''
            OR T.Nombre LIKE '%' + @Busqueda + '%'
            OR T.Identidad LIKE '%' + @Busqueda + '%'
        )
    ORDER BY T.Nombre;
END;
GO

/*
    Busca estudiantes disponibles para vincular a un tutor.
    Solo muestra estudiantes matriculados en el año activo
    y excluye los que ya están vinculados con ese tutor.
*/
CREATE OR ALTER PROCEDURE sp_MAE_BuscarEstudiantesParaVincular
    @Busqueda VARCHAR(100),
    @TutorID  INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 15
        E.EstudianteID,
        E.Nombre,
        G.NombreGrado
    FROM Estudiante E
    INNER JOIN Matricula M ON E.EstudianteID = M.EstudianteID
    INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
    INNER JOIN Grado G ON G.GradoID = S.GradoID
    WHERE
        M.Anio = (
            SELECT SUBSTRING(CicloEscolar, 1, 4)
            FROM Configuracion
            WHERE Activa = 1
        )
        AND E.Nombre LIKE '%' + @Busqueda + '%'
        AND NOT EXISTS
        (
            SELECT 1
            FROM TutorEstudiante TE
            WHERE TE.TutorID = @TutorID
              AND TE.EstudianteID = E.EstudianteID
        )
    ORDER BY E.Nombre;
END;
GO


/* =========================================================
   GRADOS Y SECCIONES
   ========================================================= */

/*
    Crea un nuevo grado académico.
    Valida que no exista otro grado con el mismo nombre.
    Devuelve el ID generado.
*/
CREATE OR ALTER PROCEDURE spMAE_crearGrados
    @Nombre VARCHAR(100),
    @Nivel  VARCHAR(50),
    @Estado INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS (
            SELECT 1
            FROM Grado
            WHERE UPPER(LTRIM(RTRIM(NombreGrado))) = UPPER(LTRIM(RTRIM(@Nombre)))
        )
        BEGIN
            THROW 50006, 'ERROR: Ya existe este grado.', 1;
        END;

        INSERT INTO Grado (NombreGrado, Nivel, Estado)
        VALUES (@Nombre, @Nivel, @Estado);

        SELECT SCOPE_IDENTITY() AS GradoID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END;
GO

/*
    Crea una nueva sección para un grado específico.
    Valida que no exista la misma letra repetida dentro del mismo grado.
*/
CREATE OR ALTER PROCEDURE spMAE_CrearSecciones
    @GradoID INT,
    @Letra   VARCHAR(10),
    @Turno   VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS (
            SELECT 1
            FROM Seccion
            WHERE GradoID = @GradoID
              AND UPPER(LTRIM(RTRIM(Letra))) = UPPER(LTRIM(RTRIM(@Letra)))
        )
        BEGIN
            THROW 50006, 'ERROR: Ya existe esta sección para el grado seleccionado.', 1;
        END;

        INSERT INTO Seccion (GradoID, Letra, Turno)
        VALUES (@GradoID, @Letra, @Turno);

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END;
GO

/*
    Lista los grados registrados.
    Permite filtrar por nivel y realizar búsqueda por nombre del grado.
*/
CREATE OR ALTER PROCEDURE spMAE_ListarGrados
    @Nivel    VARCHAR(50) = '',
    @Busqueda VARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        g.GradoID,
        g.NombreGrado,
        g.Nivel,
        CASE WHEN g.Estado = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado
    FROM Grado g
    WHERE (@Nivel = '' OR g.Nivel = @Nivel)
      AND (
            @Busqueda = ''
            OR g.NombreGrado LIKE '%' + @Busqueda + '%'
          )
    ORDER BY g.GradoID;
END;
GO

/*
    Lista todas las secciones pertenecientes a un grado.
*/
CREATE OR ALTER PROCEDURE spMAE_ListarSeccionesPorGrado
    @GradoID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.SeccionID,
        s.GradoID,
        s.Letra,
        s.Turno
    FROM Seccion s
    WHERE s.GradoID = @GradoID
    ORDER BY s.Letra;
END;
GO

/*
    Elimina una sección por su ID.
*/
CREATE OR ALTER PROCEDURE spMAE_EliminarSeccion
    @SeccionID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        DELETE FROM Seccion
        WHERE SeccionID = @SeccionID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END;
GO


/* =========================================================
   CONSULTAS SUELTAS / PRUEBAS
   ========================================================= */

/*
    Obtiene las letras de secciones asignadas a un docente
    según un grado específico.
*/
SELECT DISTINCT
    S.Letra
FROM CargaAcademica CA
INNER JOIN Seccion S ON CA.SeccionID = S.SeccionID
INNER JOIN Grado G ON S.GradoID = G.GradoID
INNER JOIN Docente D ON CA.DocenteID = D.DocenteID
WHERE D.UsuarioID = 4
  AND G.NombreGrado = 'PRIMERO'
ORDER BY S.Letra;
GO


/* =========================================================
   REPORTES DE REUNIONES DOCENTE
   ========================================================= */

/*
    Devuelve el historial de reuniones de un docente.
    Permite filtrar opcionalmente por mes, año y estado.
*/
CREATE OR ALTER PROCEDURE spMAE_RepReunionesDocenteHistorial
    @docenteID INT,
    @mes       INT = 0,
    @anio      INT = 0,
    @estado    VARCHAR(20) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ReunionID,
        R.FechaHora,
        E.Nombre,
        CONCAT(G.NombreGrado, ' ', S.Letra) AS GradoSeccion,
        R.Tema,
        R.MedioDifusion,
        R.Estado
    FROM Reunion R
    INNER JOIN Estudiante E
        ON R.EstudianteID = E.EstudianteID
    INNER JOIN Matricula M
        ON M.EstudianteID = E.EstudianteID
       AND M.Anio = YEAR(R.FechaHora)
    INNER JOIN Seccion S
        ON M.SeccionID = S.SeccionID
    INNER JOIN Grado G
        ON S.GradoID = G.GradoID
    WHERE
        R.DocenteID = @docenteID
        AND (@mes = 0 OR MONTH(R.FechaHora) = @mes)
        AND (@anio = 0 OR YEAR(R.FechaHora) = @anio)
        AND (@estado = '' OR R.Estado = @estado)
    ORDER BY
        R.FechaHora DESC,
        E.Nombre ASC;
END;
GO

/*
    Lista los meses disponibles en los que un docente tiene reuniones.
    Se puede filtrar por año.
*/
CREATE OR ALTER PROCEDURE spMAE_RepMesesReunionesDocente
    @docenteID INT,
    @anio      INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        MONTH(R.FechaHora) AS MesNumero
    FROM Reunion R
    WHERE
        R.DocenteID = @docenteID
        AND (@anio = 0 OR YEAR(R.FechaHora) = @anio)
    ORDER BY MesNumero;
END;
GO

/*
    Lista los años disponibles en los que un docente tiene reuniones.
    Útil para llenar combo de ciclo o año.
*/
CREATE OR ALTER PROCEDURE spMAE_RepAniosReunionesDocente
    @docenteID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        YEAR(R.FechaHora) AS Anio
    FROM Reunion R
    WHERE
        R.DocenteID = @docenteID
    ORDER BY Anio DESC;
END;
GO


/* =========================================================
   CONSULTAS DE APOYO / DEPURACIÓN
   ========================================================= */

/*
    Lista los años distintos registrados en la tabla Reunion.
*/
SELECT DISTINCT YEAR(FechaHora) AS Anio
FROM Reunion
ORDER BY Anio DESC;
GO

/*
    Muestra todas las reuniones ordenadas de más reciente a más antigua.
*/
SELECT *
FROM Reunion
ORDER BY FechaHora;
GO

/*
    Analiza el año calendario y el ciclo académico real
    al que pertenece cada reunión.
*/
SELECT
    ReunionID,
    FechaHora,
    MONTH(FechaHora) AS Mes,
    YEAR(FechaHora) AS AnioCalendario,
    CASE 
        WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora)
        ELSE YEAR(FechaHora) - 1
    END AS AnioInicioCiclo,
    CONCAT(
        CASE 
            WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora)
            ELSE YEAR(FechaHora) - 1
        END,
        '-',
        CASE 
            WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora) + 1
            ELSE YEAR(FechaHora)
        END
    ) AS CicloAcademicoReal,
    DocenteID,
    EstudianteID,
    Estado
FROM Reunion
ORDER BY FechaHora DESC;
GO

/*
    Lista docentes registrados.
*/
SELECT DocenteID, Nombre
FROM Docente
ORDER BY DocenteID;
GO

/*
    Verifica la relación entre una reunión y la matrícula del estudiante,
    comparando año calendario vs año de matrícula.
*/
SELECT
    R.ReunionID,
    R.EstudianteID,
    R.FechaHora,
    YEAR(R.FechaHora) AS AnioCalendario,
    CASE 
        WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
        ELSE YEAR(R.FechaHora) - 1
    END AS AnioInicioCiclo,
    M.MatriculaID,
    M.Anio AS AnioMatricula
FROM Reunion R
LEFT JOIN Matricula M
    ON M.EstudianteID = R.EstudianteID
WHERE R.ReunionID = 631
ORDER BY M.Anio DESC;
GO

/*
    Obtiene el nombre del docente asociado a una reunión específica.
*/
SELECT
    D.Nombre
FROM Reunion R
INNER JOIN Docente D
    ON R.DocenteID = D.DocenteID
WHERE R.ReunionID = 34;
GO



--sp para ingressar asistencias permite insertar y update (editar)

create or alter procedure sp_Ingresar_Asistencias_v2
    @grado varchar(100),
    @seccion varchar(50),
    @fecha date,
    @Asignatura varchar(20),
    @Estado varchar(50),
    @observacion varchar(200) = null,
    @Estudiante varchar(100)
as
begin
    set nocount on;
    set xact_abort on;

    begin try
        begin transaction;

        declare 
            @EstudianteID int,
            @CargaID int;

        -- Validar estado
        if @Estado not in ('PRESENTE', 'AUSENTE', 'JUSTIFICADO', 'TARDE')
        begin
            throw 50001, 'Solo se permite PRESENTE, AUSENTE, JUSTIFICADO o TARDE.', 1;
        end;

        -- Buscar estudiante
        select @EstudianteID = E.EstudianteID
        from Estudiante E
        where E.Nombre = @Estudiante;

        if @EstudianteID is null
        begin
            throw 50002, 'No se encontró el estudiante.', 1;
        end;

        -- Buscar carga correcta
        select top 1 @CargaID = CA.CargaID
        from CargaAcademica CA
        inner join Asignatura A on A.AsignaturaID = CA.AsignaturaID
        inner join Seccion S on S.SeccionID = CA.SeccionID
        inner join Grado G on G.GradoID = S.GradoID
        where A.Nombre = @Asignatura
          and S.Letra = @seccion
          and G.NombreGrado = @grado;

        if @CargaID is null
        begin
            throw 50003, 'No se encontró la carga académica para el grado, sección y asignatura enviados.', 1;
        end;

        -- Si ya existe, actualiza
        if exists (
            select 1
            from Asistencia
            where EstudianteID = @EstudianteID
              and CargaID = @CargaID
              and Fecha = @fecha
        )
        begin
            update Asistencia
               set Estado = @Estado,
                   Observacion = @observacion
             where EstudianteID = @EstudianteID
               and CargaID = @CargaID
               and Fecha = @fecha;
        end
        else
        begin
            insert into Asistencia
                (EstudianteID, CargaID, Fecha, Estado, Observacion)
            values
                (@EstudianteID, @CargaID, @fecha, @Estado, @observacion);
        end;

        commit transaction;
    end try
    begin catch
        if @@trancount > 0
            rollback transaction;
        throw;
    end catch
end;
go

--sp de calificaciones semanales
create or alter procedure spMAE_Calificaciones_semanales_experimental
    @Grado varchar(50),
    @parcial int,
    @Seccion varchar(10),
    @Asignatura varchar(70),
    @fecha_inicial date,
    @fecha_final date,
    @Estudiante varchar(100) = null
as
begin
    set nocount on;

    select
        E.EstudianteID,
        E.Nombre as Estudiante,
        G.NombreGrado as Grado,
        S.Letra as Seccion,
        A.Descripcion,
        ASI.Nombre as Asignatura,
        A.Parcial,
        A.Valor,
        C.Nota as calificacion,
        C.Fecha
    from Calificacion C
    inner join Actividad A
        on C.ActividadID = A.ActividadID
    inner join CargaAcademica CA
        on A.CargaID = CA.CargaID
    inner join Seccion S
        on CA.SeccionID = S.SeccionID
    inner join Grado G
        on S.GradoID = G.GradoID
    inner join Estudiante E
        on C.EstudianteID = E.EstudianteID
    inner join Asignatura ASI
        on CA.AsignaturaID = ASI.AsignaturaID
    where G.NombreGrado = @Grado
      and A.Parcial = @parcial
      and S.Letra = @Seccion
      and ASI.Nombre = @Asignatura
      and C.Fecha between @fecha_inicial and @fecha_final
      and (@Estudiante is null or E.Nombre like '%' + @Estudiante + '%')
    order by
        E.Nombre,
        A.Descripcion,
        C.Fecha;
end;
go

--Control para el reporte mensual de asistencia.
create or alter procedure spMAE_Asistencias_por_Grado
    @fecha_inicial date,
    @fecha_final date,
    @Docente int,
    @Estudiante varchar(100) = null,
    @grado varchar(100),
    @Seccion varchar(10)
as
begin
    set nocount on;

    ;with CargasFiltradas as
    (
        select
            CA.CargaID,
            CA.SeccionID,
            G.NombreGrado,
            S.Letra
        from CargaAcademica CA
        inner join Seccion S
            on CA.SeccionID = S.SeccionID
        inner join Grado G
            on S.GradoID = G.GradoID
        inner join Docente D
            on CA.DocenteID = D.DocenteID
        where D.UsuarioID = @Docente
          and G.NombreGrado = @grado
          and S.Letra = @Seccion
    )
    select
        E.EstudianteID,
        E.Nombre as Estudiante,
        CF.NombreGrado as Grado,
        CF.Letra as Seccion,
        A.Fecha,
        A.Estado,
        A.Observacion
    from Matricula M
    inner join Estudiante E
        on E.EstudianteID = M.EstudianteID
    inner join Seccion S
        on S.SeccionID = M.SeccionID
    inner join Grado G
        on G.GradoID = S.GradoID
    inner join CargasFiltradas CF
        on CF.SeccionID = M.SeccionID
    left join Asistencia A
        on A.EstudianteID = E.EstudianteID
       and A.CargaID = CF.CargaID
       and A.Fecha between @fecha_inicial and @fecha_final
    where G.NombreGrado = @grado
      and S.Letra = @Seccion
      and (@Estudiante is null or E.Nombre like '%' + @Estudiante + '%')
    order by
        E.Nombre,
        A.Fecha;
end;
go