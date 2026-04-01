use AgroLinkDB




--=================================
		--MATRICULA
--=================================

go
----**********************FUNCION PARA CREAR NOMBRE DE USUARIO 
CREATE OR ALTER FUNCTION dbo.fMAE_CrearNombreDeUsuario (@nombreCompleto VARCHAR(100))
RETURNS VARCHAR(100)
AS
BEGIN
    DECLARE @usuario VARCHAR(100), @duplicado int;

    WITH Partes AS (
        SELECT 
            value,
            ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS rn,
            COUNT(*) OVER() AS total
        FROM STRING_SPLIT(@nombreCompleto, ' ')
    )
    SELECT  
        @usuario =
        CASE 
            -- 4 palabras: 2 nombres + 2 apellidos
            WHEN total = 4 THEN
                UPPER(MAX(CASE WHEN rn = 1 THEN value END)) + '.' +
                UPPER(MAX(CASE WHEN rn = total - 1 THEN value END))

            -- otros casos con mas o menos nombres o apellidos
            ELSE  
                UPPER(MAX(CASE WHEN rn = 1 THEN value END)) + '.' +
                UPPER(MAX(CASE WHEN rn = total THEN value END))
        END
    FROM Partes group by total;

	--VALIDA SI YA EXISTE PARA ASIGNARLE UNA NUMERACION: Ejem>> NOMBRE.APELLIDO1, ..2, ..3 
	SELECT @duplicado = COUNT(*) FROM Usuario WHERE Usuario LIKE @usuario + '%' ;

	IF @duplicado > 0 
		SET @usuario = @usuario + CAST(@duplicado AS VARCHAR)
		
    RETURN @usuario;
END;

--SELECT dbo.fMAE_CrearNombreDeUsuario('Ana Garcia')

go



--**********************FUNCION DE MATRICULA

CREATE OR ALTER PROCEDURE spMAE_Matricular 
--DATOS ESTUDIANTE
@nombreEst  VARCHAR(100), @fechaNacimiento date, @sexo char, @dniEst VARCHAR(20),
@direccionEst VARCHAR(255), @telEst VARCHAR(20), @mano VARCHAR(15), @alergia VARCHAR(255),@imagen VARCHAR(255),
@gradoID int,  @seccionID varchar, --LA SECCION ES LA LETRA (A, B)
--DATOS TUTOR 1
@nombreTut1 VARCHAR(100), @dniTut1 VARCHAR(20), @telTut1 VARCHAR(20), @lugTrabTut1 VARCHAR(150), 
@correoTut1 VARCHAR(150), @parentescoTut1 VARCHAR(50),
--DATOS TUTOR 2
@nombreTut2 VARCHAR(100) = NULL , @dniTut2 VARCHAR(20) = NULL, @telTut2 VARCHAR(20) = NULL, @lugTrabTut2 VARCHAR(150)= NULL , 
@correoTut2 VARCHAR(150) = NULL,  @parentescoTut2 VARCHAR(50) = NULL

AS
BEGIN
	begin transaction
		declare @err int = 0,  @duplicado int = 0; 
		declare @usuario varchar(100) ;

		--===============
			--TUTORES
		--===============  
		--VALIDAR USUARIO DUPLICADO
		SELECT  @duplicado = COUNT(*)
		FROM Usuario WHERE Correo = @correoTut1;

		IF @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 50006, 'ERROR: Ya existe un usuario con el correo del tutor 1.', 1;
		END;

		--CREAR USUARIO TUTOR 1
		set @usuario =  dbo.fMAE_CrearNombreDeUsuario(@nombreTut1);
		
		INSERT INTO USUARIO (Usuario, Correo, Password, Rol)
		VALUES(@usuario, @correoTut1, 'Tuto123*', 'TUTOR');
		IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;
		
		--VALIDAR TUTOR DUPLICADO
		SELECT @duplicado = COUNT(*)
		FROM Tutor WHERE Identidad = @dniTut1
		
		IF @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 50007, 'ERROR: Ya existe un tutor con el DNI del tutor 1.', 1;
		END;

		--CREAR TUTOR 1
		INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, Lugartrabajo)
		SELECT TOP 1  UsuarioID, UPPER(@nombreTut1), @dniTut1, @telTut1, UPPER(@parentescoTut1), UPPER(@lugTrabTut1)
		FROM Usuario WHERE Usuario = @usuario
		IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


		IF @nombreTut2 IS NOT NULL
		BEGIN
				--VALIDAR USUARIO DUPLICADO
			SELECT  @duplicado = COUNT(*)
			FROM Usuario WHERE Correo = @correoTut2;

			IF @duplicado > 0
			BEGIN
				ROLLBACK;
				THROW 50008, 'ERROR: Ya existe un usuario con el correo del tutor 2.', 1;
			END;

			--CREAR USUARIO TUTOR 2
			set @usuario =  dbo.fMAE_CrearNombreDeUsuario(@nombreTut2);
		
			INSERT INTO USUARIO (Usuario, Correo, Password, Rol)
			VALUES(@usuario, @correoTut2, 'Tuto123*', 'TUTOR');
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;
		
			--VALIDAR TUTOR DUPLICADO
			SELECT @duplicado = COUNT(*)
			FROM Tutor WHERE Identidad = @dniTut2
		
			IF @duplicado > 0
			BEGIN
				ROLLBACK;
				THROW 50009, 'ERROR: Ya existe un tutor con el DNI del tutor 2.', 1;
			END;

			--CREAR TUTOR 1
			INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, Lugartrabajo)
			SELECT TOP 1  UsuarioID, UPPER(@nombreTut2), @dniTut2, @telTut2, UPPER(@parentescoTut2), UPPER(@lugTrabTut2)
			FROM Usuario WHERE Usuario = @usuario
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;
		END;

		--=================  
			--ESTUDIANTES
		--=================

		--VALIDAR ESTUDIANTE DUPLICADO
		SELECT @duplicado = COUNT(*)
		FROM Estudiante WHERE Identidad = @dniEst
		
		IF @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 500010, 'ERROR: Ya existe un estudiante con el DNI ingresado.', 1;
		END;

		--CREAR ESTUDIANTE
		INSERT INTO Estudiante (Nombre, Sexo, Identidad, Direccion, Telefono, FechaNacimiento, Mano, Alergia, Imagen)
		VALUES (UPPER(@nombreEst), UPPER(@sexo), @dniEst, UPPER(@direccionEst), @telEst, @fechaNacimiento, UPPER(@mano), UPPER(@alergia), @imagen)
		IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;

		--RELACIONAR TUTOR CON ESTUDIANTE
		INSERT INTO TutorEstudiante (TutorID, EstudianteID)
		SELECT T0.TutorID, T1.EstudianteID
		FROM Tutor T0 , Estudiante T1
		WHERE T0.Identidad IN (@dniTut1, @dniTut2)
		AND T1.Identidad = @dniEst
		IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


		--=================  
			--MATRICULA
		--=================
		-- !!!***** el anio se deja asi o por temas del proyecto lo cambio a por defecto 2025 ??
		INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
		SELECT EstudianteID, S.SeccionID , GETDATE(), YEAR(GETDATE()) 
		FROM Estudiante E, Seccion S
		WHERE E.Identidad = @dniEst
		AND S.GradoID = @gradoID  AND S.Letra = @seccionID
		IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;

	IF @err = 0 
	COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;
END;

go


--DATOS DE PRUEBA
begin transaction

EXEC spMAE_Matricular
-- DATOS ESTUDIANTE
@nombreEst = 'Juan Carlos Perez Lopez',
@fechaNacimiento = '2010-05-15',
@sexo = 'M',
@dniEst = '0101-2018-00069', --0801201012345
@direccionEst = 'Colonia Centro, San Pedro Sula',
@telEst = '9999-9999',
@mano = 'Derecha',
@alergia = 'Ninguna',
@imagen = 'juan.jpg',
@gradoID = 13,
@seccionID = 'A',

-- DATOS TUTOR 1
@nombreTut1 = 'Maria Martinez Lopez ',
@dniTut1 = '0801199012345',
@telTut1 = '8888-8888',
@lugTrabTut1 = 'Banco Atlántida',
@correoTut1 = 'maria.lopez@gmail.com',
@parentescoTut1 = 'Madre',

-- DATOS TUTOR 2 (opcional)
@nombreTut2 = 'Carlos Francisco Perez Lopez',
@dniTut2 = '0107-1980-10201',
@telTut2 = '7777-7777',
@lugTrabTut2 = 'Empresa XYZ',
@correoTut2 = 'carlos.perez1@gmail.com',
@parentescoTut2 = 'Padre';


rollback

--VERIFICAR
select M.Fecha, M.Anio, E.Nombre, E.Identidad, G.NombreGrado, S.Letra, T.Nombre, T.Identidad , U.Usuario, U.Correo
from Matricula M
INNER JOIN Estudiante E ON M.EstudianteID = E.EstudianteID
INNER JOIN TutorEstudiante TE ON E.EstudianteID = TE.EstudianteID
INNER JOIN Tutor T ON T.TutorID = TE.TutorID
INNER JOIN Usuario U ON T.UsuarioID = U.UsuarioID
INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
INNER JOIN Grado G ON S.GradoID = G.GradoID
WHERE M.MatriculaID = 81
order by matriculaid desc




--=======================================================================



--=================================
		--REUNIONES
--=================================

go

CREATE OR ALTER PROCEDURE spMAE_CrearReunion @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50)
AS
BEGIN
	begin transaction
		declare @error int = 0 ,  @err int = 0;  

		SELECT @error = COUNT(*)
		FROM Reunion 
		WHERE DocenteID = @docenteID 
		AND FechaHora = @fechaHora

		IF @error = 0
		BEGIN
			INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
			VALUES (@docenteID, @estudianteID, @fechaHora, @tema, @medioDifusion, 'PROGRAMADA')
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50001, 'ERROR: Ya existe una reunión programada en la fecha y hora elegida', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;

END;

go

--PRUEBA CON Excepcion:
--EXEC spMAE_CrearReunion 6 , 3, '2026-04-01 9:00:00','Seguimiento Bajo desempeño en Clase de Grammar','VIDEOLLAMADA'

GO


CREATE OR ALTER PROCEDURE spMAE_EditarReunion @reunionID int,  @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50), @estado varchar(20)
AS
BEGIN
	begin transaction

		declare @currentStatus varchar(20),  @err int = 0;

		select @currentStatus = Estado  
		from Reunion where ReunionID = @reunionID

		IF ( @currentStatus = 'PROGRAMADA' )
		BEGIN
			UPDATE Reunion SET DocenteID = @docenteID , EstudianteID = @estudianteID, 
			FechaHora = @fechaHora , Tema = @tema, MedioDifusion = @medioDifusion, Estado = @estado
			Where ReunionID = @reunionID;
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50002, 'ERROR: La reunion debe estar programada para editarla', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;

END;

go
--EXEC spMAE_EditarReunion 5,7	,5, '2026-04-05 8:35:00', 'Uso adecuado de tecnología',	'VIDEOLLAMADA',	'PROGRAMADA'

-- PRUEBA CON Excepcion:   
--EXEC spMAE_EditarReunion 3,	6	,3	, '2026-05-01 9:00:00','Bajo desempeño en Clase de Grammar','VIDEOLLAMADA',	'REALIZADA'

go




--=================================
		--ACTAS
--=================================


-- Trae datos para pantalla "ACTA DE REUNION"
CREATE OR ALTER PROCEDURE spMAE_DetalleReunionActa @reunionID int
AS
BEGIN
	SELECT R.DocenteID, R.Estado, R.FechaHora, G.NombreGrado, S.Letra, R.Tema, R.MedioDifusion, A.Acuerdos, A.Observaciones
	FROM Reunion R
	LEFT JOIN Acta A ON R.ReunionID = A.ReunionID
	INNER JOIN Matricula M ON M.EstudianteID = R.EstudianteID AND M.Anio = YEAR(R.FechaHora)
	INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
	INNER JOIN Grado G ON S.GradoID = G.GradoID
	WHERE R.ReunionID = @reunionID
END

--exec spMAE_DetalleReunionActa @reunionID = 1

GO


CREATE OR ALTER PROCEDURE spMAE_CrearActa @reunionID int,  @fechaActa datetime, @acuerdos VARCHAR(MAX), 
@observaciones VARCHAR(255)
AS
BEGIN
	begin transaction
		declare @fechaReu date , @estadoReu varchar(20)	, @err int = 0; 

		SELECT @fechaReu = FechaHora, @estadoReu = Estado
		FROM Reunion 
		WHERE ReunionID = @reunionID 

		IF @estadoReu <> 'PROGRAMADA'
		BEGIN
			ROLLBACK;
			THROW 50003, 'ERROR: La reunión debe estar programada para crear acta',1;
		END;

		IF @fechaActa >= @fechaReu 
		BEGIN
			INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
			VALUES (@reunionID, @fechaActa, @acuerdos, @observaciones)
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

			UPDATE Reunion SET Estado = 'REALIZADA'
			WHERE ReunionID = @reunionID
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 
		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50004, 'ERROR: La fecha del acta no puede ser menor a la fecha de reunión', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;
		

END;

go
--exec spMAE_CrearActa 11, '2026-04-01', 'CLASES DE REFORZAMIENTO', 'NECESITA CONCENTRARSE'

go
SELECT * FROM Reunion
SELECT * FROM ACTA



















