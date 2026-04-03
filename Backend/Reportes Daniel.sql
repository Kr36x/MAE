
--===================================
			--REPORTES
--===================================


--1. FICHA DE MATRÍCULA INDIVIDUAL (HISTÓRICO Y VIGENTE)


	--DATOS ESTUDIANTE
            --CAMBIOS:   cambiar NombreGrado por GradoID y agregar letra de seccion

CREATE OR ALTER PROCEDURE spMAE_RepFichaMatricula @estudianteID int, @matriculaID int
AS
BEGIN

	SELECT 
		M.MatriculaID, 
		E.EstudianteID, 
		E.Nombre, 
		E.FechaNacimiento, 
		E.Sexo, 
		E.Identidad, 
		E.Mano, 
		G.GradoId,
		E.Alergia,
		E.Telefono,
		E.Direccion,
		E.Imagen, 
		M.Fecha,
        S.LETRA
	FROM 
		Matricula M 
		INNER JOIN Estudiante E ON M.EstudianteID = E.EstudianteID
		INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
		INNER JOIN Grado G ON S.GradoID = G.GradoID
	WHERE 
		E.EstudianteID = @estudianteID 
		AND M.MatriculaID = @matriculaID;

END;

	--EXEC spMAE_RepFichaMatricula 1,1

	EXEC spMAE_RepFichaMatricula 13,1

go





	--DATOS TUTOR

        --CAMBIOS : agregar parentesco y correo
CREATE OR ALTER PROCEDURE spMAE_TraeTutoresxEstudiante @estudianteID int
AS
BEGIN
	SELECT T.TutorID, T.Nombre, T.Identidad, T.Telefono, T.Lugartrabajo, T.Parentesco , U.Correo
	FROM Tutor T
	INNER JOIN TutorEstudiante TE ON T.TutorID = TE.TutorID
    INNER JOIN Usuario U on T.UsuarioID = U.UsuarioID
	WHERE TE.EstudianteID = @estudianteID
END;

GO
EXEC spMAE_TraeTutoresxEstudiante 9

go






--4. DETALLE DE REUNIONES DE PADRES POR DOCENTE MENSUAL

CREATE OR ALTER PROCEDURE spMAE_RepReunionesMensuales @docenteID int, @mes int, @anio int
AS
BEGIN

	SELECT 
		R.FechaHora , 
		E.Nombre, 
		CONCAT(G.NombreGrado, CONCAT(' ', S.Letra )) as GradoSeccion, 
		R.Tema, 
		R.MedioDifusion, 
		R.Estado
	FROM 
		Reunion R
		--INNER JOIN Acta A ON R.ReunionID = A.ReunionID
		INNER JOIN Estudiante E ON R.EstudianteID = E.EstudianteID
		INNER JOIN Matricula M ON M.EstudianteID = E.EstudianteID AND M.Anio = (SELECT SUBSTRING(CicloEscolar,1,4) FROM Configuracion WHERE Activa = 1)
		INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
		INNER JOIN Grado G ON S.GradoID = G.GradoID
	WHERE 
		R.DocenteID = @docenteID 
		AND MONTH(R.FechaHora) = @mes 
		AND YEAR(R.FechaHora) = @anio

    ORDER BY R.FechaHora, E.Nombre;
END;

EXEC spMAE_RepReunionesMensuales 1, 3, 2026
--EXEC spMAE_RepReunionesMensuales 13, 3, 2026



GO

	--5. Reportes Globales de Rendimiento Institucional


CREATE OR ALTER PROCEDURE spMAE_RepGlobalesRend @nivel nvarchar(20)
AS
BEGIN
    WITH Promedios AS (
        SELECT E.EstudianteID, G.GradoID,G.NombreGrado,S.Letra,

            -- promedio por estudiante
            CAST(
                SUM(CAl.Nota) * 100.0 /
                NULLIF(SUM(AC.Valor), 0)
            AS DECIMAL(5,2)) AS Promedio, AC.Parcial

        FROM Estudiante E 
        JOIN Matricula M ON M.EstudianteID = E.EstudianteID AND M.Anio = (SELECT SUBSTRING(CicloEscolar,1,4) FROM Configuracion WHERE Activa = 1)
        JOIN CargaAcademica CA ON CA.SeccionID = M.SeccionID AND CA.Anio = (SELECT SUBSTRING(CicloEscolar,1,4) FROM Configuracion WHERE Activa = 1)
        JOIN Seccion S ON S.SeccionID = CA.SeccionID
        JOIN Grado G ON G.GradoID = S.GradoID

        INNER JOIN Actividad AC ON AC.CargaID = CA.CargaID
        INNER JOIN Calificacion CAl ON CAl.ActividadID = AC.ActividadID AND CAl.EstudianteID = E.EstudianteID

        WHERE G.Nivel = @nivel 

        GROUP BY E.EstudianteID,G.GradoID,G.NombreGrado,S.Letra , AC.Parcial
    )

    SELECT 
        NombreGrado, Letra,
        CAST(AVG(Promedio) AS DECIMAL(5,2)) AS PromedioGrado,
        CONCAT(SUM(CASE WHEN Promedio > 85 THEN 1 ELSE 0 END), CONCAT( ' '  , 'EST.') )AS Excelencia , Parcial
    FROM Promedios
    GROUP BY GradoID,NombreGrado,Letra,  Parcial
    ORDER BY GradoID;
END;



exec spMAE_RepGlobalesRend @nivel = 'BASICA'

go


    --6. Boleta de Calificaciones Finales por Parcial
 
CREATE OR ALTER PROCEDURE spMAE_BoletaParcial @periodo int, @gradoID int, @letraSeccion varchar, @anio int
AS
BEGIN


    WITH  
    ConfigActiva AS (
        SELECT TOP 1 Periodo
        FROM Configuracion
        WHERE Periodo = @periodo AND Anio = @anio
        ORDER BY Anio DESC, Periodo DESC
    ),
    
    Promedios AS (
        SELECT 
            A.Nombre AS Asignatura,  D.Nombre AS Docente,

            -- promedio por estudiante en esa clase
            CAST(
                SUM(CAl.Nota) * 100.0 /
                NULLIF(SUM(AC.Valor), 0)
            AS DECIMAL(5,2)) AS Promedio

        FROM Estudiante E 
        JOIN Matricula M ON M.EstudianteID = E.EstudianteID 
        JOIN CargaAcademica CA ON CA.SeccionID = M.SeccionID 
        JOIN Seccion S ON S.SeccionID = CA.SeccionID
        JOIN Grado G ON G.GradoID = S.GradoID
        JOIN Asignatura A ON A.AsignaturaID = CA.AsignaturaID
        JOIN Docente D ON D.DocenteID = CA.DocenteID

        CROSS JOIN ConfigActiva C

        LEFT JOIN Actividad AC ON AC.CargaID = CA.CargaID AND AC.Parcial = C.Periodo 
        LEFT JOIN Calificacion CAl ON CAl.ActividadID = AC.ActividadID AND CAl.EstudianteID = E.EstudianteID

        WHERE CA.Anio = @anio  AND S.Letra = @letraSeccion AND G.GradoID = @gradoID
        GROUP BY A.Nombre,D.Nombre,  E.EstudianteID 
    )

    SELECT 
        Asignatura,Docente, CAST(AVG(Promedio) AS DECIMAL(5,2)) AS PromedioClase,
        CASE  
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) >= 85 THEN 'EXCELENTE'
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) >= 70 AND CAST(AVG(Promedio) AS DECIMAL(5,2)) < 85  THEN 'MEDIO'
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) < 70 THEN 'CRITICO'
        END AS "Estado"

    FROM Promedios
    GROUP BY Asignatura,Docente
    ORDER BY Asignatura;



END;
go

  exec spMAE_BoletaParcial @periodo= 3, @gradoID =  17, @letraSeccion = 'A', @anio =2025
  select * from BoletaDetalle


go


	--7. Asignación de Docentes y Cantidad de Alumnos por Docente

	--DOCENTES ASIGNADOS
CREATE OR ALTER PROCEDURE spMAE_RepDistribCargaDoc @grado int, @seccion int, @anio int
AS
BEGIN

	SELECT 
		D.DocenteID, D.Nombre, D.Identidad, D.Estado, COUNT(A.AsignaturaID) as Cantidad
	FROM CargaAcademica CA
	INNER JOIN Docente D ON CA.DocenteID = D.DocenteID
	INNER JOIN Asignatura A ON CA.AsignaturaID = A.AsignaturaID
	INNER JOIN Seccion S ON CA.SeccionID = S.SeccionID
	INNER JOIN Grado G ON S.GradoID = G.GradoID  
	WHERE G.GradoID = @grado AND S.SeccionID = @seccion and CA.Anio = @anio
	GROUP BY D.DocenteID, D.Nombre, D.Identidad, D.Estado
    ORDER BY D.Nombre

END;
exec spMAE_RepDistribCargaDoc 13,7,2025

GO

	--ASIGNATURAS ASIGNADAS
CREATE OR ALTER PROCEDURE spMAE_TraeAsignaturaxDocxSecc @docente int, @seccion int
AS
BEGIN
	SELECT A.AsignaturaID, A.Nombre
	FROM Asignatura A
	INNER JOIN CargaAcademica CA ON A.AsignaturaID = CA.AsignaturaID
	WHERE CA.DocenteID = @docente AND CA.SeccionID = @seccion
    ORDER BY A.Nombre

END;

go
exec spMAE_TraeAsignaturaxDocxSecc 8,7

go





    --8.  PROYECCIÓN DE DESERCIÓN Y RETENCIÓN ESTUDIANTIL EN EL AÑO

go
CREATE OR ALTER VIEW vMAE_RepProyDesercionGen 
AS

    
    -- SUBQUERIES
    WITH ConfigActiva AS (
        SELECT CAST(SUBSTRING(CicloEscolar,1,4) AS INT) AS Anio 
        FROM Configuracion 
        WHERE Activa = 1
    ),
    Ausencias AS (
        SELECT A.EstudianteID, COUNT(*) AS Inasistencias 
        FROM Asistencia A 
        CROSS JOIN ConfigActiva C
        WHERE A.Estado = 'AUSENTE' 
          AND YEAR(A.Fecha) IN (C.Anio, C.Anio + 1)
        GROUP BY A.EstudianteID
    )


    --QUERY PRINCIPAL
    SELECT TOP 10 AU.EstudianteID,E.Nombre,G.NombreGrado AS Grado,S.Letra AS Seccion,AU.Inasistencias,
        CAST(
            SUM(CAl.Nota) * 100.0 /
            NULLIF(SUM(AC.Valor), 0)
        AS DECIMAL(5,2)) AS PromedioAnual

    FROM Ausencias AU
    JOIN Estudiante E ON E.EstudianteID = AU.EstudianteID
    JOIN Matricula M ON M.EstudianteID = AU.EstudianteID AND M.Anio = (SELECT Anio FROM ConfigActiva)

    JOIN CargaAcademica CA ON CA.SeccionID = M.SeccionID 
    JOIN Seccion S ON S.SeccionID = CA.SeccionID
    JOIN Grado G ON G.GradoID = S.GradoID

    LEFT JOIN Actividad AC ON AC.CargaID = CA.CargaID
    LEFT JOIN Calificacion CAl ON CAl.ActividadID = AC.ActividadID AND CAl.EstudianteID = AU.EstudianteID

    GROUP BY AU.EstudianteID,E.Nombre,G.NombreGrado,S.Letra,AU.Inasistencias
    ORDER BY AU.Inasistencias DESC;

go



SELECT * FROM vMAE_RepProyDesercionGen 

SELECT COUNT(*) FROM Asistencia  WHERE Estado = 'AUSENTE' and Estudianteid = 44
 

go


CREATE OR ALTER VIEW vMAE_RepProyDesercionDet 
AS
    WITH TopEstudiantes AS (
        SELECT TOP 10 EstudianteID
        FROM vMAE_RepProyDesercionGen
        ORDER BY Inasistencias DESC
    ),
    ConfigActiva AS (
        SELECT CAST(SUBSTRING(CicloEscolar,1,4) AS INT) AS Anio 
        FROM Configuracion 
        WHERE Activa = 1
    )

    SELECT 
        A.EstudianteID,
        AA.Nombre AS Asignatura,
        COUNT(*) AS Inasistencias
    FROM Asistencia A
    JOIN TopEstudiantes T ON T.EstudianteID = A.EstudianteID
    JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
    JOIN Asignatura AA ON CA.AsignaturaID = AA.AsignaturaID
    CROSS JOIN ConfigActiva C

    WHERE 
        A.Estado = 'AUSENTE'
        AND YEAR(A.Fecha) IN (C.Anio, C.Anio + 1)

    GROUP BY 
        A.EstudianteID,
        AA.Nombre


        go




select * from vMAE_RepProyDesercionGen a 
inner join vMAE_RepProyDesercionDet b on a.EstudianteID = b.EstudianteID
order by a.EstudianteID 


