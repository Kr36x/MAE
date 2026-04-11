--==============================
	--UTILIDADES
--==============================


use AgroLinkDB
--TABLA PREVIA A REPORTES PARA FICHA MATRICULA
CREATE OR ALTER PROCEDURE spMAE_TraeEstudiantesConGrado @nombre nvarchar(100), @grado int , @anio int
AS
BEGIN
	SELECT E.EstudianteID, E.Nombre, M.MatriculaID, G.GradoID, G.NombreGrado AS Grado, S.Letra as Seccion
	FROM Estudiante E
	INNER JOIN Matricula M ON E.EstudianteID = M.EstudianteID
	INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
	INNER JOIN GRADO G ON S.GradoID = G.GradoID
	WHERE E.Nombre LIKE '%' + @nombre + '%' AND M.Anio = @anio AND G.GradoID = @grado;

END;

--EXEC spTraeEstudiantesConGrado 'D',   11 , 2024

go
-- Llenar el datagrip Estudiante
CREATE OR ALTER VIEW vMAE_LlnearEstudiantes 
AS
	SELECT Nombre,Grado,
	FROM Estudiante
go

select * from Estudiante
-- PARA USAR EN COMBOBOXES
CREATE OR ALTER VIEW vMAE_TraeGrados 
AS
	SELECT GradoID, NombreGrado
	FROM Grado
go

SELECT * FROM vMAE_TraeGrados
order by GradoID
GO

select * from Usuario

CREATE OR ALTER PROCEDURE spMAE_TraeAsignaturas @gradoID int , @anio int
AS
BEGIN
	SELECT A.AsignaturaID, A.Nombre
	FROM Asignatura A
	INNER JOIN CargaAcademica CA ON A.AsignaturaID = CA.AsignaturaID
	INNER JOIN Seccion S ON CA.SeccionID = S.SeccionID
	INNER JOIN Grado G ON G.GradoID = S.GradoID
	WHERE G.GradoID = @gradoID AND CA.Anio = @anio
	ORDER BY A.Nombre
END;

exec spMAE_TraeAsignaturas 14, 2025


GO


CREATE OR ALTER VIEW vMAE_Meses
AS 
	SELECT 1 AS MESID ,  'ENERO' AS MES
	UNION ALL
	SELECT 2 AS MESID ,  'FEBRERO' AS MES
	UNION ALL
	SELECT 3 AS MESID ,  'MARZO' AS MES
	UNION ALL
	SELECT 4 AS MESID ,  'ABRIL' AS MES
	UNION ALL
	SELECT 5 AS MESID ,  'MAYO' AS MES
	UNION ALL
	SELECT 6 AS MESID ,  'JUNIO' AS MES
	UNION ALL
	SELECT 7 AS MESID ,  'JULIO' AS MES
	UNION ALL
	SELECT 8 AS MESID ,  'AGOSTO' AS MES
	UNION ALL
	SELECT 9 AS MESID ,  'SEPTIEMBRE' AS MES
	UNION ALL
	SELECT 10 AS MESID ,  'OCTUBRE' AS MES
	UNION ALL
	SELECT 11 AS MESID ,  'NOVIEMBRE' AS MES
	UNION ALL
	SELECT 12 AS MESID ,  'DICIEMBRE' AS MES
	
go
--SELECT * FROM vMAE_Meses
go

--------------- Reportes Bryan
use AgroLinkDB
----------------------------------------------------------------
CREATE VIEW vMAE_EstudianteGradoAnio
AS
SELECT 
    e.EstudianteID,
    e.Nombre AS NombreEstudiante,
    g.NombreGrado,
    g.Nivel,
    m.Anio AS AnioAcademico
FROM Matricula m
INNER JOIN Estudiante e ON m.EstudianteID = e.EstudianteID
INNER JOIN Seccion s ON m.SeccionID = s.SeccionID
INNER JOIN Grado g ON s.GradoID = g.GradoID;
GO
--SELECT * FROM vMAE_EstudianteGradoAnio
------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_BuscarEstudiantes
    @Nombre VARCHAR(100) = NULL,
    @Anio INT = NULL,
    @Grado VARCHAR(100) = NULL
AS
BEGIN
    SELECT *
    FROM vMAE_EstudianteGradoAnio
    WHERE (@Nombre IS NULL OR NombreEstudiante LIKE '%' + @Nombre + '%')
      AND (@Anio IS NULL OR AnioAcademico = @Anio)
      AND (@Grado IS NULL OR NombreGrado LIKE '%' + @Grado + '%')
    ORDER BY NombreEstudiante ASC;
END;
GO

exec spMAE_BuscarEstudiantes
exec spMAE_TraeMatriculaPorEstudiante 1
exec spMAE_TraeTutoresxEstudiante 25
exec spMAE_RepFichaMatricula 1, 13

-----------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_DetalleEstudianteCompleto
    @EstudianteID INT
AS
BEGIN
    SELECT 
        e.EstudianteID,
        e.Nombre AS NombreEstudiante,
        e.Identidad AS IdentidadEstudiante,
        e.Sexo,
        e.FechaNacimiento,
        e.Direccion,
        e.Telefono AS TelefonoEstudiante,
        e.Mano,
        e.Alergia,
        CASE 
            WHEN e.Estado = 1 THEN 'ACTIVO'
            ELSE 'INACTIVO'
        END AS Estado,

        g.NombreGrado,
        m.Anio AS AnioAcademico,

        padre.Nombre AS NombrePadre,
        padre.Identidad AS IdentidadPadre,
        padre.Telefono AS TelefonoPadre,
        padre.LugarTrabajo AS LugarTrabajoPadre,

        madre.Nombre AS NombreMadre,
        madre.Identidad AS IdentidadMadre,
        madre.Telefono AS TelefonoMadre,
        madre.LugarTrabajo AS LugarTrabajoMadre

    FROM Estudiante e
    LEFT JOIN Matricula m 
        ON e.EstudianteID = m.EstudianteID
    LEFT JOIN Seccion s 
        ON m.SeccionID = s.SeccionID
    LEFT JOIN Grado g 
        ON s.GradoID = g.GradoID

    LEFT JOIN TutorEstudiante tePadre 
        ON e.EstudianteID = tePadre.EstudianteID
    LEFT JOIN Tutor padre 
        ON tePadre.TutorID = padre.TutorID 
       AND padre.Parentesco = 'PADRE'

    LEFT JOIN TutorEstudiante teMadre 
        ON e.EstudianteID = teMadre.EstudianteID
    LEFT JOIN Tutor madre 
        ON teMadre.TutorID = madre.TutorID 
       AND madre.Parentesco = 'MADRE'

    WHERE e.EstudianteID = @EstudianteID;
END
GO

EXECUTE spMAE_DetalleEstudianteCompleto 1

select*from Usuario
-------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_PromedioYExcelenciaPorNivel
    @Anio INT
AS
BEGIN
    ;WITH Promedios AS
    (
        SELECT 
            g.NombreGrado,
            b.Anio,
            b.EstudianteID,

            AVG(
                (ISNULL(d.NotaP1,0) +
                 ISNULL(d.NotaP2,0) +
                 ISNULL(d.NotaP3,0) +
                 ISNULL(d.NotaP4,0)) / 4.0
            ) AS PromedioAnual
        FROM BoletaDetalle d
        INNER JOIN Boleta b ON d.BoletaID = b.BoletaID
        INNER JOIN Matricula m ON b.EstudianteID = m.EstudianteID AND b.Anio = m.Anio
        INNER JOIN Seccion s ON m.SeccionID = s.SeccionID
        INNER JOIN Grado g ON s.GradoID = g.GradoID
        WHERE b.Anio = @Anio
        GROUP BY g.NombreGrado, b.Anio, b.EstudianteID
    )

    SELECT 
        NombreGrado,
        Anio AS AnioAcademico,
        CAST(AVG(PromedioAnual) AS INT) AS PromedioGrado,

        SUM(CASE WHEN PromedioAnual > 90 THEN 1 ELSE 0 END) AS EstudiantesExcelencia
    FROM Promedios
    GROUP BY NombreGrado, Anio

    ORDER BY 
        CASE  
            WHEN NombreGrado LIKE 'Pre-kinder%' THEN 1
			WHEN NombreGrado LIKE 'Kinder%' THEN 2
			WHEN NombreGrado LIKE 'Preparatoria%' THEN 3
			WHEN NombreGrado LIKE 'Primero%' THEN 4
            WHEN NombreGrado LIKE 'Segundo%' THEN 5
            WHEN NombreGrado LIKE 'Tercero%' THEN 6
            WHEN NombreGrado LIKE 'Cuarto%' THEN 7
            WHEN NombreGrado LIKE 'Quinto%' THEN 8
            WHEN NombreGrado LIKE 'Sexto%' THEN 9
            WHEN NombreGrado LIKE 'Séptimo%' OR NombreGrado LIKE 'Septimo%' THEN 10
            WHEN NombreGrado LIKE 'Octavo%' THEN 11
            WHEN NombreGrado LIKE 'Noveno%' THEN 12
			WHEN NombreGrado LIKE 'Decimo%' THEN 13
			WHEN NombreGrado LIKE 'Undecimo%' THEN 14
            ELSE 99
        END;
END
GO
execute spMAE_PromedioYExcelenciaPorNivel 2026
----------------------------------------------------------------------------------------
use AgroLinkDB
CREATE OR ALTER PROCEDURE spMAE_PromedioYExcelenciaPorParcial
    @Anio INT,
    @Parcial INT   -- 1, 2, 3 o 4
AS
BEGIN
    ;WITH Promedios AS
    (
        SELECT 
            g.NombreGrado,
            b.Anio,
            b.EstudianteID,

            CASE @Parcial
                WHEN 1 THEN ISNULL(d.NotaP1,0)
                WHEN 2 THEN ISNULL(d.NotaP2,0)
                WHEN 3 THEN ISNULL(d.NotaP3,0)
                WHEN 4 THEN ISNULL(d.NotaP4,0)
            END AS NotaParcial
        FROM BoletaDetalle d
        INNER JOIN Boleta b ON d.BoletaID = b.BoletaID
        INNER JOIN Matricula m ON b.EstudianteID = m.EstudianteID AND b.Anio = m.Anio
        INNER JOIN Seccion s ON m.SeccionID = s.SeccionID
        INNER JOIN Grado g ON s.GradoID = g.GradoID
        WHERE b.Anio = @Anio
    )

    SELECT 
        NombreGrado,
        Anio AS AnioAcademico,
        CAST(AVG(NotaParcial) AS INT) AS PromedioGrado,
        SUM(CASE WHEN NotaParcial > 90 THEN 1 ELSE 0 END) AS EstudiantesExcelencia
    FROM Promedios
    GROUP BY NombreGrado, Anio

    ORDER BY 
        CASE  
            WHEN NombreGrado LIKE 'Pre-kinder%' THEN 1
            WHEN NombreGrado LIKE 'Kinder%' THEN 2
            WHEN NombreGrado LIKE 'Preparatoria%' THEN 3
            WHEN NombreGrado LIKE 'Primero%' THEN 4
            WHEN NombreGrado LIKE 'Segundo%' THEN 5
            WHEN NombreGrado LIKE 'Tercero%' THEN 6
            WHEN NombreGrado LIKE 'Cuarto%' THEN 7
            WHEN NombreGrado LIKE 'Quinto%' THEN 8
            WHEN NombreGrado LIKE 'Sexto%' THEN 9
            WHEN NombreGrado LIKE 'Séptimo%' OR NombreGrado LIKE 'Septimo%' THEN 10
            WHEN NombreGrado LIKE 'Octavo%' THEN 11
            WHEN NombreGrado LIKE 'Noveno%' THEN 12
            WHEN NombreGrado LIKE 'Decimo%' THEN 13
            WHEN NombreGrado LIKE 'Undecimo%' THEN 14
            ELSE 99
        END;
END
GO
use AgroLinkDB
exec spMAE_PromedioYExcelenciaPorParcial 2026,4
----------------------------------------------------------------------------------------
select * from vMAE_EstudianteGradoAnio
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_DesempenoPorGradoAnual
    @Anio INT
AS
BEGIN
    SELECT 
        g.NombreGrado,
        CAST(AVG(
            (ISNULL(d.NotaP1,0) +
             ISNULL(d.NotaP2,0) +
             ISNULL(d.NotaP3,0) +
             ISNULL(d.NotaP4,0)) / 4.0
        ) AS INT) AS PromedioGrado
    FROM BoletaDetalle d
    INNER JOIN Boleta b ON d.BoletaID = b.BoletaID
    INNER JOIN Matricula m ON b.EstudianteID = m.EstudianteID AND b.Anio = m.Anio
    INNER JOIN Seccion s ON m.SeccionID = s.SeccionID
    INNER JOIN Grado g ON s.GradoID = g.GradoID
    WHERE b.Anio = @Anio
    GROUP BY g.NombreGrado
    ORDER BY 
        CASE 
			WHEN g.NombreGrado LIKE 'Pre-kinder%' THEN 1
			WHEN g.NombreGrado LIKE 'Kinder%' THEN 2
			WHEN g.NombreGrado LIKE 'Preparatoria%' THEN 3
            WHEN g.NombreGrado LIKE 'Primero%' THEN 4
            WHEN g.NombreGrado LIKE 'Segundo%' THEN 5
            WHEN g.NombreGrado LIKE 'Tercero%' THEN 6
            WHEN g.NombreGrado LIKE 'Cuarto%' THEN 7
            WHEN g.NombreGrado LIKE 'Quinto%' THEN 8
            WHEN g.NombreGrado LIKE 'Sexto%' THEN 9
            WHEN g.NombreGrado LIKE 'Séptimo%' OR g.NombreGrado LIKE 'Septimo%' THEN 10
            WHEN g.NombreGrado LIKE 'Octavo%' THEN 11
            WHEN g.NombreGrado LIKE 'Noveno%' THEN 12
			WHEN g.NombreGrado LIKE 'Decimo%' THEN 13
			WHEN g.NombreGrado LIKE 'Undecimo%' THEN 14
            ELSE 99
        END;
END
GO

exec spMAE_DesempenoPorGradoAnual 2026
----------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_BuscarDocentesPorGradoSeccionAnio
    @Grado      VARCHAR(50) = NULL,
    @Seccion    VARCHAR(50) = NULL,
    @Anio       INT = NULL,
    @Nombre     VARCHAR(100) = NULL
AS
BEGIN
    SELECT distinct
        d.DocenteID,
        d.Nombre,
        d.Identidad,
        d.Especialidad
    FROM vMAE_CargarDocentes d
    LEFT JOIN CargaAcademica ad 
        ON d.DocenteID = ad.DocenteID
    LEFT JOIN Seccion s 
        ON ad.SeccionID = s.SeccionID
    LEFT JOIN Grado g 
        ON s.GradoID = g.GradoID
    WHERE 
        (@Grado IS NULL OR g.NombreGrado = @Grado)
        AND (@Seccion IS NULL OR s.Letra = @Seccion)
        AND (@Anio IS NULL OR ad.Anio = @Anio)
        AND (@Nombre IS NULL OR d.Nombre LIKE '%' + @Nombre + '%')
    ORDER BY d.Nombre;
END
GO
exec spMAE_BuscarDocentesPorGradoSeccionAnio
-------------------------------------
CREATE OR ALTER VIEW vMAE_CargarDocentes 
AS
	SELECT DocenteID, Nombre,Identidad,Especialidad
	FROM Docente
go
select * from vMAE_CargarDocentes

select * from docente

-------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_TraeAsignaturasPorDocente
    @DocenteID INT,
    @Anio INT
AS
BEGIN
    SELECT 
        A.AsignaturaID,
        A.Nombre AS Asignatura,
        G.NombreGrado

    FROM CargaAcademica CA
    INNER JOIN Asignatura A ON CA.AsignaturaID = A.AsignaturaID
    INNER JOIN Seccion S ON CA.SeccionID = S.SeccionID
    INNER JOIN Grado G ON S.GradoID = G.GradoID
    WHERE CA.DocenteID = @DocenteID
      AND CA.Anio = @Anio
    ORDER BY A.Nombre;
END;
GO

exec spMAE_TraeAsignaturasPorDocente 3, 2026
--------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_CargaAcademicaDocente
    @Anio INT
AS
BEGIN
    SELECT 
        d.DocenteID,
        d.Nombre,
        COUNT(*) AS TotalClases
    FROM CargaAcademica ca
    INNER JOIN Docente d ON ca.DocenteID = d.DocenteID
    WHERE ca.Anio = @Anio
    GROUP BY d.DocenteID, d.Nombre
    ORDER BY d.Nombre;
END;
GO

exec spMAE_CargaAcademicaDocente 2025

CREATE OR ALTER PROCEDURE spMAE_BuscarFichaMatriculaPorIdentidad
@Identidad VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        E.EstudianteID,
        E.Nombre,
        E.Sexo,
        E.Identidad,
        E.Direccion,
        E.Telefono,
        E.FechaNacimiento,
        E.Mano,
        E.Alergia,
        E.Imagen,

        M.MatriculaID,
        M.Fecha AS FechaMatricula,
        M.Anio AS MatriculaAnio,

        S.SeccionID,
        S.Letra AS SeccionLetra,
        S.Turno,
        S.Aula,

        G.GradoID,
        G.NombreGrado,
        G.Nivel
    FROM Estudiante E
    LEFT JOIN Matricula M ON M.EstudianteID = E.EstudianteID
    LEFT JOIN Seccion S ON S.SeccionID = M.SeccionID
    LEFT JOIN Grado G ON G.GradoID = S.GradoID
    WHERE E.Identidad = @Identidad;

    SELECT 
        T.TutorID,
        T.Nombre,
        T.Identidad,
        T.Telefono,
        T.LugarTrabajo,
        T.Parentesco
    FROM Estudiante E
    INNER JOIN TutorEstudiante TE ON TE.EstudianteID = E.EstudianteID
    INNER JOIN Tutor T ON T.TutorID = TE.TutorID
    WHERE E.Identidad = @Identidad;
END;

-------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_ObtenerSecciones
AS
BEGIN
    SET NOCOUNT ON;

    WITH CTE AS
    (
        SELECT 
            SeccionID,
            Letra,
            ROW_NUMBER() OVER (PARTITION BY Letra ORDER BY SeccionID) AS rn
        FROM Seccion
    )
    SELECT 
        SeccionID,
        Letra
    FROM CTE
    WHERE rn = 1
    ORDER BY Letra;
END;

exec spMAE_ObtenerSecciones

select * from estudiante
-------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_ObtenerSexo
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 'M' AS Codigo, 'MASCULINO' AS Descripcion
    UNION ALL
    SELECT 'F', 'FEMENINO';
END;

exec spMAE_ObtenerSexo

CREATE OR ALTER PROCEDURE spMAE_CargaAcademicaDocenteSeccion
    @Anio INT,
    @Grado VARCHAR(50) = NULL,
    @Seccion VARCHAR(10) = NULL
AS
BEGIN
    SELECT 
        d.DocenteID,
        d.Nombre,
        COUNT(*) AS TotalClases
    FROM CargaAcademica ca
    INNER JOIN Docente d ON ca.DocenteID = d.DocenteID
    INNER JOIN Seccion s ON ca.SeccionID = s.SeccionID
    INNER JOIN Grado g ON s.GradoID = g.GradoID
    WHERE ca.Anio = @Anio
      AND (@Grado IS NULL OR g.NombreGrado = @Grado)
      AND (@Seccion IS NULL OR s.Letra = @Seccion)
    GROUP BY d.DocenteID, d.Nombre
    ORDER BY d.Nombre;
END;
GO
exec spMAE_CargaAcademicaDocenteSeccion 2026,'DECIMO',A
----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spMAE_TraeAsignaturasPorDocenteSeccion
    @DocenteID INT,
    @Anio INT,
    @Grado VARCHAR(50) = NULL,
    @Seccion VARCHAR(10) = NULL
AS
BEGIN
    SELECT 
        A.AsignaturaID,
        A.Nombre AS Asignatura,
        G.NombreGrado,
        S.Letra AS Seccion
    FROM CargaAcademica CA
    INNER JOIN Asignatura A ON CA.AsignaturaID = A.AsignaturaID
    INNER JOIN Seccion S ON CA.SeccionID = S.SeccionID
    INNER JOIN Grado G ON S.GradoID = G.GradoID
    WHERE CA.DocenteID = @DocenteID
      AND CA.Anio = @Anio
      AND (@Grado IS NULL OR G.NombreGrado = @Grado)
      AND (@Seccion IS NULL OR S.Letra = @Seccion)
    ORDER BY A.Nombre;
END;
GO

exec spMAE_PromedioYExcelenciaPorParcial 2026, 1

--------------------Reportes Entradas Bryan---------------------------
CREATE OR ALTER PROCEDURE spMAE_ActualizarEstadoUsuario
    @usuarioID INT,
    @estado BIT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE Usuario
        SET Estado = @estado
        WHERE UsuarioID = @usuarioID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;

CREATE OR ALTER PROCEDURE spMAE_RepGlobalRend
    @Anio INT,
    @Parcial INT
AS
BEGIN
    WITH Promedios AS (
        SELECT 
            E.EstudianteID,
            G.GradoID,
            G.NombreGrado,
            S.Letra,

            CAST(
                SUM(CAl.Nota) * 100.0 /
                NULLIF(SUM(AC.Valor), 0)
            AS DECIMAL(5,2)) AS Promedio,

            AC.Parcial
        FROM Estudiante E
        JOIN Matricula M 
            ON M.EstudianteID = E.EstudianteID
           AND M.Anio = @Anio
        JOIN CargaAcademica CA 
            ON CA.SeccionID = M.SeccionID
           AND CA.Anio = @Anio
        JOIN Seccion S 
            ON S.SeccionID = CA.SeccionID
        JOIN Grado G 
            ON G.GradoID = S.GradoID
        JOIN Actividad AC 
            ON AC.CargaID = CA.CargaID
        JOIN Calificacion CAl 
            ON CAl.ActividadID = AC.ActividadID
           AND CAl.EstudianteID = E.EstudianteID

        WHERE AC.Parcial = @Parcial

        GROUP BY 
            E.EstudianteID,
            G.GradoID,
            G.NombreGrado,
            S.Letra,
            AC.Parcial
    )

    SELECT  
        NombreGrado,
        Letra,

        CAST(ROUND(AVG(Promedio), 0) AS INT) AS PromedioGrado,   -- ← YA REDONDEADO

        CONCAT(
            SUM(CASE WHEN Promedio > 85 THEN 1 ELSE 0 END),
            ' EST.'
        ) AS Excelencia,

        Parcial
    FROM Promedios
    GROUP BY 
        GradoID,
        NombreGrado,
        Letra,
        Parcial
    ORDER BY 
        GradoID;
END;
GO
GO
GO

EXEC spMAE_RepGlobalRend 2025,2












