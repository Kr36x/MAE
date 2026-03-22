--==============================
	--UTILIDADES
--==============================



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

--EXEC spTraeEstudiantesConGrado 'D',   22 , 2026

go




-- PARA USAR EN COMBOBOXES
CREATE OR ALTER VIEW vMAE_TraeGrados 
AS
	SELECT GradoID, NombreGrado
	FROM Grado 

go

SELECT * FROM vMAE_TraeGrados
order by GradoID
GO



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

exec spMAE_TraeAsignaturas 14, 2026


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























