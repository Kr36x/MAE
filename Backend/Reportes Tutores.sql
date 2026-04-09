
use AgroLinkDB


go

CREATE OR ALTER PROCEDURE spMAE_AsistenciaTutorEstudiante @tutorID int, @estudianteID int,  @fecha date
AS
BEGIN
	SELECT A.AsistenciaID, E.EstudianteID, E.Nombre, AA.Nombre as "Asignatura", A.Fecha, A.Estado , A.Observacion	
	FROM Asistencia A
	INNER JOIN Estudiante E ON A.EstudianteID = E.EstudianteID
	INNER JOIN TutorEstudiante TE ON E.EstudianteID = TE.EstudianteID
	INNER JOIN CargaAcademica CA ON A.CargaID = CA.CargaID
	INNER JOIN Asignatura AA ON CA.AsignaturaID = AA.AsignaturaID
	WHERE TE.TutorID = @tutorID AND A.Fecha = @fecha and  TE.EstudianteID= @estudianteID

END;

go

EXEC spMAE_AsistenciaTutorEstudiante @tutorID = 1,@estudianteID= 1, @fecha = '2026-03-02' 


go

select * from TutorEstudiante
go

CREATE OR ALTER PROCEDURE spMAE_CalificacionesTutorEstudiante @tutorID int, @estudianteID int, @anio int
AS
BEGIN	
	WITH    
    Promedios AS (
        SELECT 
            G.NombreGrado, A.Nombre AS Asignatura, AC.Parcial,

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
        JOIN TutorEstudiante TE ON E.EstudianteID = TE.EstudianteID 

        LEFT JOIN Actividad AC ON AC.CargaID = CA.CargaID 
        LEFT JOIN Calificacion CAl ON CAl.ActividadID = AC.ActividadID AND CAl.EstudianteID = E.EstudianteID

        WHERE CA.Anio = @anio  AND E.EstudianteID = @estudianteID  AND TE.TutorID = @tutorID
        GROUP BY A.Nombre,   G.NombreGrado, AC.Parcial
    )

    SELECT 
        NombreGrado, Asignatura, Parcial, CAST(AVG(Promedio) AS DECIMAL(5,2)) AS PromedioClase,
        CASE  
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) >= 85 THEN 'EXCELENTE'
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) >= 70 AND CAST(AVG(Promedio) AS DECIMAL(5,2)) < 85  THEN 'MEDIO'
            WHEN CAST(AVG(Promedio) AS DECIMAL(5,2)) < 70 THEN 'CRITICO'
        END AS "Estado"

    FROM Promedios
    GROUP BY  NombreGrado, Asignatura, Parcial
    ORDER BY Asignatura, Parcial;

	

END;

go

exec spMAE_CalificacionesTutorEstudiante @tutorID =1, @estudianteID = 1 ,  @anio = 2025

go





CREATE OR ALTER PROCEDURE spMAE_ReunionesTutorEstudiante @tutorID int, @estudianteID int, @anio int
AS
BEGIN

	SELECT DISTINCT 
		R.FechaHora , 
		E.Nombre, 
		CONCAT(G.NombreGrado, CONCAT(' ', S.Letra )) as GradoSeccion, 
        D.Nombre as "Docente",
        AA.Nombre AS "Asignatura",
		R.Tema, 
		R.MedioDifusion, 
		R.Estado
	FROM 
		Reunion R
		INNER JOIN Estudiante E ON R.EstudianteID = E.EstudianteID
		INNER JOIN Matricula M ON M.EstudianteID = E.EstudianteID AND M.Anio = @anio
		INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
		INNER JOIN Grado G ON S.GradoID = G.GradoID
        inner join TutorEstudiante TE ON TE.EstudianteID = E.EstudianteID AND TE.TutorID = @tutorID
        INNER JOIN CargaAcademica CA ON R.DocenteID = CA.DocenteID AND CA.Anio = @anio
        INNER JOIN Asignatura AA ON AA.AsignaturaID = CA.AsignaturaID
        INNER JOIN DOCENTE D ON CA.DocenteID = D.DocenteID AND R.DocenteID = D.DocenteID

	WHERE 
		E.EstudianteID= @estudianteID
		AND YEAR(R.FechaHora) in ( @anio , @anio+1)

    ORDER BY R.FechaHora, E.Nombre;
END;
go

exec spMAE_ReunionesTutorEstudiante @tutorID =1, @estudianteID = 1 ,  @anio = 2025

go


