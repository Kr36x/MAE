use AgroLinkDB








--==============================================
		--CREACION Y EDICION DE USUARIOS
--==============================================


-- TABLA DE USUARIOS
GO
CREATE OR ALTER PROCEDURE spMAE_TraeUsuarios @rol varchar(20) = '', @usuario VARCHAR(50) = '', @correo VARCHAR(150)= ''
AS
BEGIN

	SELECT U.UsuarioID, U.Usuario, U.Correo,  U.Rol,
	CASE 
		WHEN T.Nombre IS NOT NULL THEN T.Nombre
		WHEN D.Nombre IS NOT NULL THEN D.Nombre
		ELSE A.Nombre
	END AS Vinculacion,
	U.Estado
	FROM Usuario U
	LEFT JOIN Tutor T on U.UsuarioID= T.UsuarioID
	LEFT JOIN Docente D ON U.UsuarioID = D.UsuarioID
	LEFT JOIN Admin A ON U.UsuarioID = A.UsuarioID

	WHERE U.Rol LIKE '%' + @rol + '%' AND (U.Usuario LIKE '%' + @usuario + '%'  OR U.Correo LIKE '%' + @correo + '%' )


END;

GO

--exec spMAE_TraeUsuarios '', 'MARIA', 'MARIA'



-- DETALLE DE USUARIOS


CREATE OR ALTER PROCEDURE spMAE_TraeUsuarios @usuarioID int
AS
BEGIN
	declare @rol varchar(20);

	select @rol = Rol 
	FROM Usuario
	Where UsuarioID = @usuarioID;


	IF @rol = 'Administrador'
	BEGIN
		SELECT 
		--Datos de Usuario
		U.*,
		--Datos Generales
		A.AdminID, A.Nombre, A.Identidad, A.Sexo, A.Telefono, A.Direccion, A.Posicion
		FROM Usuario U
		INNER JOIN Admin A ON U.UsuarioID = A.UsuarioID
		WHERE U.UsuarioID = @usuarioID
	
	END;
	
	IF @rol = 'Docente'
	BEGIN
		SELECT 
		--Datos de Usuario
		U.*,
		--Datos Generales
		D.DocenteID, D.Nombre, D.Identidad, D.Sexo, D.Telefono, D.Direccion, D.FechaNacimiento, D.Especialidad
		FROM Usuario U
		INNER JOIN Docente D ON U.UsuarioID = D.UsuarioID
		WHERE U.UsuarioID = @usuarioID
	END;


	IF @rol = 'Tutor'
	BEGIN
		SELECT 
		--Datos de Usuario
		U.*,
		--Datos Generales
		T.TutorID, T.Nombre, T.Identidad, T.Telefono, T.Parentesco, T.Lugartrabajo
		FROM Usuario U
		INNER JOIN Tutor T ON U.UsuarioID = T.UsuarioID
		WHERE U.UsuarioID = @usuarioID
	END;

	

END;

go

exec spMAE_TraeUsuarios @usuarioID = 17



--	CAMBIAR ESTADO USUARIOS

go
CREATE OR ALTER PROCEDURE spMAE_CambiarEstadoUsuario @usuarioID int
AS
BEGIN
	BEGIN TRY
		begin transaction
			declare @err int = 0;

			UPDATE Usuario 
			SET Estado = CASE WHEN Estado = 1 THEN 0 ELSE 1 END
			WHERE UsuarioID = @usuarioID
	
		COMMIT TRANSACTION;

		SELECT @usuarioID;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
END;

begin transaction

exec spMAE_CambiarEstadoUsuario 4

rollback

select * from usuario





--	CREAR Y EDITAR USUARIOS 

go

CREATE OR ALTER PROCEDURE spMAE_Crear_EditarUsuario 
--Datos de Usuario
@usuario VARCHAR(50), @correo VARCHAR(150), @password VARCHAR(255), @rol VARCHAR(20) ,
--Datos generales 
@nombre varchar(100), @identidad VARCHAR(20),  @telefono VARCHAR(20) ,
--Datos Admin y Docente (AD)
@sexoAD CHAR(1) = null, @direccionAD VARCHAR(255) = null ,
--Datos Admin (A)
@posicionA VARCHAR(50) = null,
--Datos Docente (D)
@fechaNacimientoD date = null , @especialidadD VARCHAR(100) = null ,
--Datos Tutor (T)
@parentescoT VARCHAR(50) = null , @lugartrabajoT VARCHAR(150) = null,

--Para editar
@usuarioID int = null

AS
BEGIN
	BEGIN TRY
	begin transaction
		declare @duplicado INT = 0;



		IF @usuarioID IS NULL
		BEGIN
			--	CREAR USUARIO

					--VALIDACIONES
			SELECT @duplicado = COUNT(*)
			FROM USUARIO WHERE USUARIO like @usuario + '%'
			Set @usuario = Case WHEN  @duplicado > 0 then CONCAT(@usuario, @duplicado) ELSE @usuario END;


			SELECT @duplicado = COUNT(*)
			FROM USUARIO WHERE Correo = @correo
			IF @duplicado > 0 
				THROW 500012, 'ERROR: Ya existe un usuario con el correo ingresado.', 1;

				
					--NUEVO USUARIO
			INSERT INTO Usuario(Usuario, Correo,Password, Rol)
			VALUES(UPPER(@usuario), @correo, @password, @rol )
		
			SET @usuarioID = SCOPE_IDENTITY();
		

			--VALIDAR ROL
			IF @rol = 'Administrador'
			BEGIN
					--VALIDACIONES
				SELECT @duplicado = COUNT(*)
				FROM ADMIN WHERE Identidad = @identidad
				IF @duplicado > 0 
					THROW 500012, 'ERROR: Ya existe un administrador con el DNI ingresado.', 1;
					
					--NUEVO ADMIN
				INSERT INTO ADMIN (UsuarioID, Nombre, Identidad, Sexo, Telefono, Direccion, Posicion, Estado)
				VALUES(UPPER(@usuarioID), UPPER(@nombre), @identidad, @sexoAD, @telefono, UPPER(@direccionAD), UPPER(@posicionA) , 1)			
			END;
		
		
			IF @rol = 'Docente'
			BEGIN
					--VALIDACIONES
				SELECT @duplicado = COUNT(*)
				FROM Docente WHERE Identidad = @identidad
				IF @duplicado > 0 
					THROW 500012, 'ERROR: Ya existe un docente con el DNI ingresado.', 1;
					
					--NUEVO DOCENTE
				INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento,  Telefono, Direccion, Especialidad, Estado)
				VALUES(UPPER(@usuarioID), UPPER(@nombre), @identidad,  @sexoAD,@fechaNacimientoD, @telefono, UPPER(@direccionAD), UPPER(@especialidadD) , 1)	
			END;
		

			IF @rol = 'Tutor'
			BEGIN
					--VALIDACIONES
				SELECT @duplicado = COUNT(*)
				FROM Tutor WHERE Identidad = @identidad
				IF @duplicado > 0 
					THROW 500012, 'ERROR: Ya existe un tutor con el DNI ingresado.', 1;
					
					--NUEVO TUTOR
				INSERT INTO Tutor (UsuarioID, Nombre, Identidad,  Telefono, Parentesco, Lugartrabajo, Estado)
				VALUES(UPPER(@usuarioID), UPPER(@nombre), @identidad,  @telefono, UPPER(@parentescoT), UPPER(@lugartrabajoT) , 1)	
			
			END;
		
		END;
		-- EDITAR
		ELSE
		BEGIN
					--EDITAR USUARIO
			UPDATE Usuario SET Correo = @correo, Password = @password
			WHERE UsuarioID = @usuarioID

			--VALIDAR ROL
			IF @rol = 'Administrador'
			BEGIN
				
				UPDATE ADMIN SET  Telefono = @telefono, Direccion = @direccionAD, Posicion = @posicionA
				WHERE UsuarioID = @usuarioID

			END;

			IF @rol = 'Docente'
			BEGIN
				UPDATE Docente SET  Telefono = @telefono, Direccion = @direccionAD, Especialidad = @especialidadD
				WHERE UsuarioID = @usuarioID
			END;

			IF @rol = 'Tutor'
			BEGIN
				UPDATE Tutor SET  Telefono = @telefono, Lugartrabajo = @lugartrabajoT
				WHERE UsuarioID = @usuarioID
			END;
		END;


		COMMIT TRANSACTION;

		SELECT @usuarioID

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
END;

go


select * from docente

-- PRUEBAS DE CREACION Y EDICION

begin transaction

EXEC spMAE_Crear_EditarUsuario
-- Usuario
@usuario = 'ADMIN.JUAN',
@correo = 'juan.admin@gmail.com',
@password = 'Admin123*',
@rol = 'Administrador',

-- Datos generales
@nombre = 'Juan Perez',
@identidad = '0801199012345',
@telefono = '9999-9999',

-- Datos Admin/Docente
@sexoAD = 'M',
@direccionAD = 'San Pedro Sula',

-- Datos Admin
@posicionA = 'Director',

-- Docente (NULL)
@fechaNacimientoD = NULL,
@especialidadD = NULL,

-- Tutor (NULL)
@parentescoT = NULL,
@lugartrabajoT = NULL,

-- Editar
@usuarioID = NULL;

rollback


begin transaction


-- **DOCENTE

EXEC spMAE_Crear_EditarUsuario
-- Usuario
@usuario = 'JUAN.PEREZ',
@correo = 'maria.docenteQ1@gmail.com',
@password = 'Doc123*',
@rol = 'DOCENTE',

-- Datos generales
@nombre = 'Maria Lopez',
@identidad = '0107-1990-00001',
@telefono = '9911-2244',

-- Datos Admin/Docente
@sexoAD = 'M',
@direccionAD = 'Col. Centro',

-- Admin (NULL)
@posicionA = NULL,

-- Docente
@fechaNacimientoD = '3-15-1990',
@especialidadD = 'Matemáticas 1',

-- Tutor (NULL)
@parentescoT = NULL,
@lugartrabajoT = NULL,

-- Editar
@usuarioID = 4;

SELECT * FROM DOCENTE WHERE UsuarioID = 4
SELECT * FROM Usuario WHERE UsuarioID = 4




rollback


begin transaction

EXEC spMAE_Crear_EditarUsuario
-- Usuario
@usuario = 'TUTOR.CARLOS',
@correo = 'carlos.tutor@gmail.com',
@password = 'Tuto123*',
@rol = 'TUTOR',

-- Datos generales
@nombre = 'Carlos Martinez',
@identidad = '0801198012345',
@telefono = '7777-7777',

-- Admin/Docente (NULL)
@sexoAD = NULL,
@direccionAD = NULL,

-- Admin (NULL)
@posicionA = NULL,

-- Docente (NULL)
@fechaNacimientoD = NULL,
@especialidadD = NULL,

-- Tutor
@parentescoT = 'Padre',
@lugartrabajoT = 'Empresa XYZ',

-- Editar
@usuarioID = NULL;


rollback


go


--	DETALLE DE ESTUDIANTES VINCULADOS A TUTOR


CREATE OR ALTER PROCEDURE sp_MAE_EstudiantesVinculados @tutorID int 
AS
BEGIN
	SELECT TE.TutorID, TE.EstudianteID, E.Nombre, G.NombreGrado,  T.Parentesco
	FROM TutorEstudiante TE 
	INNER JOIN Tutor T ON TE.TutorID = T.TutorID
	INNER JOIN Estudiante E ON TE.EstudianteID = E.EstudianteID
	INNER JOIN Matricula M ON E.EstudianteID = M.EstudianteID 
	INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
	INNER JOIN Grado G ON G.GradoID = S.GradoID

	WHERE TE.TutorID = @tutorID AND M.Anio = (SELECT SUBSTRING(CicloEscolar,1,4) FROM Configuracion WHERE Activa = 1)


END;


exec sp_MAE_EstudiantesVinculados 87


go
--	VINCULAR TUTOR NUEVO CON ESTUDIANTE


CREATE OR ALTER PROCEDURE sp_MAE_VincularEstudianteATutor @tutorID int, @estudianteID int 
AS
BEGIN
	BEGIN TRY
	begin transaction
		declare @yaVinculado int  = 0;

		SELECT @yaVinculado = COUNT(*)
		FROM TutorEstudiante TE
		WHERE TE.TutorID = @tutorID AND TE.EstudianteID = @estudianteID 

		IF @yaVinculado > 0
			THROW 500013, 'ERROR: El tutor ya se encuentra vinculado con el estudiante.', 1;
		ELSE
		BEGIN
			INSERT INTO TutorEstudiante VALUES (@tutorID, @estudianteID)
		END;
		
		COMMIT TRANSACTION;

		SELECT @tutorID

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
END;


go


CREATE OR ALTER PROCEDURE sp_MAE_DesvincularEstudiante @tutorID int, @estudianteID int 
AS
BEGIN
	BEGIN TRY
	begin transaction
		declare @yaVinculado int  = 0;

		SELECT @yaVinculado = COUNT(*)
		FROM TutorEstudiante TE
		WHERE TE.TutorID = @tutorID AND TE.EstudianteID = @estudianteID 

		IF @yaVinculado = 0
			THROW 500013, 'ERROR: El tutor no se encuentra vinculado con el estudiante.', 1;
		ELSE
		BEGIN
			DELETE FROM TutorEstudiante WHERE TutorID = @tutorID AND EstudianteID = @estudianteID
		END;
		
		COMMIT TRANSACTION;

		SELECT @tutorID

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
END;



begin transaction

exec sp_MAE_DesvincularEstudiante 87 , 20

exec sp_MAE_EstudiantesVinculados 87

exec sp_MAE_VincularEstudianteATutor 87 , 20

exec sp_MAE_EstudiantesVinculados 87


rollback



EXEC spMAE_RepFichaMatricula 13,1


--=================================
		--MATRICULA
--=================================


-- TODO >> edicion de matricula :0

go
----**********************FUNCION PARA CREAR NOMBRE DE USUARIO  --********************** 
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

--********************** CREAR Y EDITAR MATRICULA --********************** 

CREATE OR ALTER PROCEDURE spMAE_Matricular 
--DATOS ESTUDIANTE
@nombreEst  VARCHAR(100), @fechaNacimiento date, @sexo char, @dniEst VARCHAR(20),
@direccionEst VARCHAR(255), @telEst VARCHAR(20), @mano VARCHAR(15), @alergia VARCHAR(255),@imagen VARCHAR(255),
@gradoID int,  @seccionID varchar, --LA SECCION ES LA LETRA (A, B)
--DATOS TUTOR 1
@nombreTut1 VARCHAR(100), @dniTut1 VARCHAR(20), @telTut1 VARCHAR(20), @lugTrabTut1 VARCHAR(150), 
@correoTut1 VARCHAR(150), @parentescoTut1 VARCHAR(50), --El parentesco y correo deben agregarse al formulario
--DATOS TUTOR 2
@nombreTut2 VARCHAR(100) = NULL , @dniTut2 VARCHAR(20) = NULL, @telTut2 VARCHAR(20) = NULL, @lugTrabTut2 VARCHAR(150)= NULL , 
@correoTut2 VARCHAR(150) = NULL,  @parentescoTut2 VARCHAR(50) = NULL

--PARA EDITAR
, @matriculaID int = null
AS
BEGIN
	BEGIN TRY
		begin transaction
			declare @err int = 0,  @duplicado int = 0; 
			declare @usuario varchar(100), @usuarioID int,  @estudianteID int , @tutorID int,  @gradoIdActual int ;



			IF @matriculaID IS NULL
			BEGIN
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


				IF @dniTut2 IS NOT NULL
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
					THROW 50010, 'ERROR: Ya existe un estudiante con el DNI ingresado.', 1;
				END;

				--CREAR ESTUDIANTE
				INSERT INTO Estudiante (Nombre, Sexo, Identidad, Direccion, Telefono, FechaNacimiento, Mano, Alergia, Imagen)
				VALUES (UPPER(@nombreEst), UPPER(@sexo), @dniEst, UPPER(@direccionEst), @telEst, @fechaNacimiento, UPPER(@mano), UPPER(@alergia), @imagen)

				--RELACIONAR TUTOR CON ESTUDIANTE
				INSERT INTO TutorEstudiante (TutorID, EstudianteID)
				SELECT T0.TutorID, T1.EstudianteID
				FROM Tutor T0 , Estudiante T1
				WHERE T0.Identidad IN (@dniTut1, @dniTut2)
				AND T1.Identidad = @dniEst


				--=================  
					--MATRICULA
				--=================
				INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
				SELECT EstudianteID, S.SeccionID ,GETDATE(), year(GETDATE()) -- DATEADD(YEAR, -1, GETDATE()), '2025' 
				FROM Estudiante E, Seccion S
				WHERE E.Identidad = @dniEst
				AND S.GradoID = @gradoID  AND S.Letra = @seccionID
				
				SET @matriculaID = SCOPE_IDENTITY();

			END
			ELSE
			BEGIN
				
				--EDITAR TUTOR
				SELECT @tutorID = T.TutorID, @usuarioID = T.UsuarioID
				FROM Tutor T
				WHERE Identidad = @dniTut1 
				
				UPDATE Tutor SET Telefono = @telTut1, Lugartrabajo = UPPER(@lugTrabTut1)
				WHERE TutorID = @tutorID
				UPDATE Usuario SET Correo = @correoTut1
				WHERE UsuarioID = @usuarioID

				--VALIDA SI EXISTE TUTOR 2 PARA EDITARLO
				IF @dniTut2 IS NOT NULL
				BEGIN
					SELECT @tutorID = T.TutorID, @usuarioID = T.UsuarioID
					FROM Tutor T
					WHERE Identidad = @dniTut2 
				
					UPDATE Tutor SET Telefono = @telTut2, Lugartrabajo = UPPER(@lugTrabTut2)
					WHERE TutorID = @tutorID
					UPDATE Usuario SET Correo = @correoTut2
					WHERE UsuarioID = @usuarioID
				END


				--EDITAR ESTUDIANTE
				SELECT @estudianteID = M.EstudianteID,  @gradoIdActual = S.GradoID
				FROM Matricula M 
				INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
				WHERE M.MatriculaID = @matriculaID

				UPDATE Estudiante SET Mano = UPPER(@mano), Alergia = UPPER(@alergia), Telefono = @telEst, Direccion = UPPER(@direccionEst), Imagen = @imagen
				WHERE EstudianteID = @estudianteID


				-- VALIDAR SI EL ESTUDIANTE CURSARA OTRO GRADO PARA CREAR NUEVA MATRICULA
				IF @gradoID > @gradoIdActual
				BEGIN 
					
					INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
					SELECT EstudianteID, S.SeccionID ,  GETDATE(), year(GETDATE())
					FROM Estudiante E, Seccion S
					WHERE E.EstudianteID = @estudianteID
					AND S.GradoID = @gradoID  AND S.Letra = @seccionID

					SET @matriculaID = SCOPE_IDENTITY();
				END
			END


			COMMIT TRANSACTION;
			SELECT @matriculaID ;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
END;

go


--DATOS DE PRUEBA
begin transaction

EXEC spMAE_Matricular
-- DATOS ESTUDIANTE
@nombreEst = 'Juan Carlos Perez Lopez',
@fechaNacimiento = '2010-05-15',
@sexo = 'M',
@dniEst = '0101-2016-00011', ----existente: 00011 nuevo: 00069
@direccionEst = 'Colonia Centro, San Pedro Sula',
@telEst = '9999-9999',
@mano = 'Derecha',
@alergia = 'Ninguna',
@imagen = 'juan.jpg',
@gradoID = 17,
@seccionID = 'A',

-- DATOS TUTOR 1
@nombreTut1 = 'Maria Martinez Lopez ',
@dniTut1 = '0107-1982-10002', --dni existente 0107-1980-10002
@telTut1 = '8888-8888',
@lugTrabTut1 = 'Banco Atlántida',
@correoTut1 = 'maria.lopez@gmail.com',
@parentescoTut1 = 'Madre',

-- DATOS TUTOR 2 (opcional)
@nombreTut2 = 'Carlos Francisco Perez Lopez',
@dniTut2 = '0107-1980-10001', --dni existente 0107-1980-10001
@telTut2 = '7777-7777',
@lugTrabTut2 = 'Empresa XYZ',
@correoTut2 = 'carlos.perez1@gmail.com',
@parentescoTut2 = 'Padre'
,

@matriculaID = 2



rollback

--VERIFICAR
select M.MatriculaID,  M.Fecha, M.Anio, E.Nombre, E.Identidad,  e.Direccion, G.NombreGrado, S.Letra, T.Nombre, T.Identidad , U.Usuario, U.Correo, t.Telefono 
from Matricula M
INNER JOIN Estudiante E ON M.EstudianteID = E.EstudianteID
INNER JOIN TutorEstudiante TE ON E.EstudianteID = TE.EstudianteID
INNER JOIN Tutor T ON T.TutorID = TE.TutorID
INNER JOIN Usuario U ON T.UsuarioID = U.UsuarioID
INNER JOIN Seccion S ON M.SeccionID = S.SeccionID
INNER JOIN Grado G ON S.GradoID = G.GradoID
WHERE 
e.EstudianteID = 1

M.MatriculaID = 89
order by matriculaid desc

select *from Estudiante where identidad = '0101-2016-00011'

select * from tutor where Identidad = '0107-1982-10001'

--********************** EDITAR MATRICULA --********************** 
select * from grado






































--=======================================================================



--=================================
		--REUNIONES
--=================================

go

CREATE OR ALTER PROCEDURE spMAE_CrearReunion @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50)
AS
BEGIN
	BEGIN TRY

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

			END
			ELSE
			BEGIN
				ROLLBACK;
				THROW 50001, 'ERROR: Ya existe una reunión programada en la fecha y hora elegida', 1;
			END;

			COMMIT TRANSACTION;

			SELECT SCOPE_IDENTITY();
	END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

		THROW;
    END CATCH;

END;

go

--PRUEBA CON Excepcion:

begin transaction
--EXEC spMAE_CrearReunion 6 , 3, '2026-04-01 9:00:00','Seguimiento Bajo desempeño en Clase de Grammar','VIDEOLLAMADA'


rollback
GO


CREATE OR ALTER PROCEDURE spMAE_EditarReunion @reunionID int,  @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50), @estado varchar(20)
AS
BEGIN
	BEGIN TRY


		begin transaction

			declare @currentStatus varchar(20),  @err int = 0;

			select @currentStatus = Estado  
			from Reunion where ReunionID = @reunionID

			IF ( @currentStatus = 'PROGRAMADA' )
			BEGIN
				UPDATE Reunion SET DocenteID = @docenteID , EstudianteID = @estudianteID, 
				FechaHora = @fechaHora , Tema = @tema, MedioDifusion = @medioDifusion, Estado = @estado
				Where ReunionID = @reunionID;

			END
			ELSE
			BEGIN
				ROLLBACK;
				THROW 50002, 'ERROR: La reunion debe estar programada para editarla', 1;
			END;

			COMMIT TRANSACTION;

			SELECT @reunionID;

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;

END;

go

begin transaction
--EXEC spMAE_EditarReunion 5,7	,5, '2026-04-05 8:35:00', 'Uso adecuado de tecnología',	'VIDEOLLAMADA',	'PROGRAMADA'


begin transaction
-- PRUEBA CON Excepcion:   
--EXEC spMAE_EditarReunion 3,	6	,3	, '2026-05-01 9:00:00','Bajo desempeño en Clase de Grammar','VIDEOLLAMADA',	'REALIZADA'



rollback
go



CREATE OR ALTER PROCEDURE spMAE_CancelarReunion @reunionID int
AS
BEGIN
	BEGIN TRY
		begin transaction
			declare @estado varchar(20),@err int = 0;
	
			SELECT  @estado = Estado
			FROM Reunion WHERE ReunionID = @reunionID

			IF @estado <> 'REALIZADA'
			BEGIN
				UPDATE Reunion SET Estado = 'CANCELADA'
				WHERE ReunionID = @reunionID

			END
			ELSE
			BEGIN
				ROLLBACK;
				THROW 50002, 'ERROR: no puede cancelar reuniones que ya han sido realizadas', 1;
			END

			COMMIT TRANSACTION

			SELECT @reunionID
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;

END;

GO

--PARA PRUEBAS
begin transaction

	--exec spMAE_CancelarReunion 2;
	
	select * from reunion
	where ReunionID = 2

rollback

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
	BEGIN TRY
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

				UPDATE Reunion SET Estado = 'REALIZADA'
				WHERE ReunionID = @reunionID
			END
			ELSE
			BEGIN
				ROLLBACK;
				THROW 50011, 'ERROR: La fecha del acta no puede ser menor a la fecha de reunión', 1;
			END;

			COMMIT TRANSACTION;

			SELECT SCOPE_IDENTITY();
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH;
		

END;

go

BEGIN TRANSACTION
exec spMAE_CrearActa 11, '2026-04-01', 'CLASES DE REFORZAMIENTO', 'NECESITA CONCENTRARSE'



ROLLBACK
go
SELECT * FROM Reunion
SELECT * FROM ACTA



















