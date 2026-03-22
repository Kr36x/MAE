use AgroLinkDB



--=========================
-- TABLA GRADO
--=========================

SELECT * FROM Grado
INSERT INTO GRADO (NombreGrado, Nivel)
VALUES('PRE-KINDER', 'PRE-BASICA'),
('KINDER', 'PRE-BASICA'),
('PREPARATORIA', 'PRE-BASICA'),
('PRIMERO', 'BASICA'),
('SEGUNDO', 'BASICA'),
('TERCERO', 'BASICA'),
('CUARTO', 'BASICA'),
('QUINTO', 'BASICA'),
('SEXTO', 'BASICA'),
('SEPTIMO', 'SECUNDARIA'),
('OCTAVO', 'SECUNDARIA'),
('NOVENO', 'SECUNDARIA'),
('DECIMO', 'SECUNDARIA'),
('UNDECIMO', 'SECUNDARIA')


--=========================
-- TABLA SECCION
--=========================

INSERT INTO Seccion (GradoID, Letra, Turno, Aula, Anio)
VALUES(10, 'A', 'MATUTINO', 1, 2026),
(10, 'B', 'MATUTINO', 2, 2026),
(11, 'A', 'MATUTINO', 3, 2026),
(11, 'B', 'MATUTINO', 4, 2026),
(12, 'A', 'MATUTINO', 5, 2026),
(12, 'B', 'MATUTINO', 6, 2026),
(13, 'A', 'MATUTINO', 7, 2026),
(13, 'B', 'MATUTINO', 8, 2026),
(14, 'A', 'MATUTINO', 9, 2026),
(14, 'B', 'MATUTINO', 10, 2026),
(15, 'A', 'MATUTINO', 11, 2026),
(15, 'B', 'MATUTINO', 12, 2026),
(16, 'A', 'MATUTINO', 13, 2026),
(16, 'B', 'MATUTINO', 14, 2026),
(17, 'A', 'MATUTINO', 15, 2026),
(17, 'B', 'MATUTINO', 16, 2026),
(18, 'A', 'MATUTINO', 17, 2026),
(18, 'B', 'MATUTINO', 18, 2026),
(19, 'A', 'MATUTINO', 19, 2026),
(19, 'B', 'VESPERTINO', 20, 2026),
(20, 'A', 'MATUTINO', 21, 2026),
(20, 'B', 'VESPERTINO', 22, 2026),
(21, 'A', 'MATUTINO', 23, 2026),
(21, 'B', 'VESPERTINO', 24, 2026),
(22, 'A', 'MATUTINO', 25, 2026),
(22, 'B', 'VESPERTINO', 26, 2026),
(23, 'A', 'MATUTINO', 27, 2026),
(23, 'B', 'VESPERTINO', 28, 2026)




--=========================
-- TABLA USUARIO
--=========================
select * from USUARIO
	--ADMINISTRADORAS

INSERT INTO Usuario (Usuario, Correo, Password, Rol, Estado) VALUES
('maria.martinez', 'maria.martinez@gmail.com', 'Admin123*', 'Administrador', 1),
('carol.cerrato', 'carol.cerrato@gmail.com', 'Admin123*', 'Administrador', 1),
('laura.lopez', 'laura.lopez@gmail.com', 'Admin123*', 'Administrador', 1);


    --DOCENTES 
INSERT INTO Usuario (Usuario, Correo, Password, Rol, Estado) VALUES
('juan.perez', 'juan.perez@gmail.com', 'Docente123*', 'Docente', 1),
('ana.garcia', 'ana.garcia@gmail.com', 'Docente123*', 'Docente', 1),
('carlos.ramirez', 'carlos.ramirez@gmail.com', 'Docente123*', 'Docente', 1),
('luisa.fernandez', 'luisa.fernandez@gmail.com', 'Docente123*', 'Docente', 1),
('miguel.torres', 'miguel.torres@gmail.com', 'Docente123*', 'Docente', 1),
('sofia.herrera', 'sofia.herrera@gmail.com', 'Docente123*', 'Docente', 1),
('andres.morales', 'andres.morales@gmail.com', 'Docente123*', 'Docente', 1),
('elena.vargas', 'elena.vargas@gmail.com', 'Docente123*', 'Docente', 1),
('daniel.reyes', 'daniel.reyes@gmail.com', 'Docente123*', 'Docente', 1),
('patricia.castro', 'patricia.castro@gmail.com', 'Docente123*', 'Docente', 1),
('roberto.mejia', 'roberto.mejia@gmail.com', 'Docente123*', 'Docente', 1),
('gabriela.navarro', 'gabriela.navarro@gmail.com', 'Docente123*', 'Docente', 1),
('fernando.alvarado', 'fernando.alvarado@gmail.com', 'Docente123*', 'Docente', 1);


    --TUTORES
INSERT INTO Usuario (Usuario, Correo, Password, Rol, Estado) VALUES
-- 1
('carlos.perez', 'carlos.perez@gmail.com', 'Tuto123*', 'Tutor', 1),
('marta.perez', 'marta.perez@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 2
('jorge.garcia', 'jorge.garcia@gmail.com', 'Tuto123*', 'Tutor', 1),
('silvia.garcia', 'silvia.garcia@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 3
('ricardo.ramirez', 'ricardo.ramirez@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 4
('rosa.fernandez', 'rosa.fernandez@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 5
('oscar.torres', 'oscar.torres@gmail.com', 'Tuto123*', 'Tutor', 1),
('veronica.torres', 'veronica.torres@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 6
('hector.herrera', 'hector.herrera@gmail.com', 'Tuto123*', 'Tutor', 1),
('diana.herrera', 'diana.herrera@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 7
('luis.morales', 'luis.morales@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 8
('karla.vargas', 'karla.vargas@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 9
('sergio.reyes', 'sergio.reyes@gmail.com', 'Tuto123*', 'Tutor', 1),
('andrea.reyes', 'andrea.reyes@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 10
('miguel.castro', 'miguel.castro@gmail.com', 'Tuto123*', 'Tutor', 1),
('lucia.castro', 'lucia.castro@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 11
('monica.mejia', 'monica.mejia@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 12
('rafael.navarro', 'rafael.navarro@gmail.com', 'Tuto123*', 'Tutor', 1),
-- 13
('carlos.alvarado', 'carlos.alvarado@gmail.com', 'Tuto123*', 'Tutor', 1),
('gloria.alvarado', 'gloria.alvarado@gmail.com', 'Tuto123*', 'Tutor', 1);




--=========================
-- TABLA ADMIN
--=========================
select * from admin d
inner join Usuario u on d.UsuarioID = u.UsuarioID

INSERT INTO Admin ( UsuarioID,Nombre,Identidad,Sexo,Telefono,Direccion,Posicion,Estado)
--Por si toca hacer delete, asi se crea de un solo sin necesidad de andar editando usuarioid.
SELECT 
    U.UsuarioID, 'Maria Martinez','0107199012345','F','9876-1234','Barrio Venecia, Tela, Atlántida','Directora',  1
FROM Usuario U
WHERE U.Usuario = 'maria.martinez';


INSERT INTO Admin ( UsuarioID,Nombre,Identidad,Sexo,Telefono,Direccion,Posicion,Estado)
SELECT 
    U.UsuarioID, 'Carol Cerrato','0107199209876','F','9988-4455','Col. San Juan, Tela, Atlántida','Administradora', 1
FROM Usuario U
WHERE U.Usuario = 'carol.cerrato';


INSERT INTO Admin ( UsuarioID,Nombre,Identidad,Sexo,Telefono,Direccion,Posicion,Estado)
SELECT 
    U.UsuarioID,'Laura Lopez','0107199501122','F','9777-8899','Col. La Independencia, Tela, Atlántida','Secretaria',1
FROM Usuario U
WHERE U.Usuario = 'laura.lopez';





--=========================
-- TABLA DOCENTE
--=========================
select * from docente d
inner join Usuario u on d.UsuarioID = u.UsuarioID

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Juan Perez', '0107199000001', 'M', '1990-03-15', '9911-2233', 'Col. La Independencia, Tela, Atlántida', 'Matematicas', 1
FROM Usuario WHERE Usuario = 'juan.perez';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Ana Garcia', '0107199100002', 'F', '1991-07-20', '9922-3344', 'Col. San Juan, Tela, Atlántida', 'Lengua y Literatura', 1
FROM Usuario WHERE Usuario = 'ana.garcia';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Carlos Ramirez', '0107198800003', 'M', '1988-01-10', '9933-4455', 'Barrio Venecia, Tela, Atlántida', 'Fisica', 1
FROM Usuario WHERE Usuario = 'carlos.ramirez';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Luisa Fernandez', '0107199300004', 'F', '1993-09-05', '9944-5566', 'Barrio Venecia, Tela, Atlántida', 'Lengua y Literatura', 1
FROM Usuario WHERE Usuario = 'luisa.fernandez';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Miguel Torres', '0107198700005', 'M', '1987-11-25', '9955-6677', 'Col. San Juan, Tela, Atlántida', 'Historia', 1
FROM Usuario WHERE Usuario = 'miguel.torres';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Sofia Herrera', '0107199200006', 'F', '1992-04-18', '9966-7788', 'Col. La Independencia, Tela, Atlántida', 'Ingles', 1
FROM Usuario WHERE Usuario = 'sofia.herrera';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Andres Morales', '0107198900007', 'M', '1989-06-12', '9977-8899', 'Col. Trejo, Tela, Atlántida', 'Informatica', 1
FROM Usuario WHERE Usuario = 'andres.morales';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Elena Vargas', '0107199400008', 'F', '1994-02-22', '9988-9900', 'Col. Trejo, Tela, Atlántida', 'Ingles', 1
FROM Usuario WHERE Usuario = 'elena.vargas';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Daniel Reyes', '0107198600009', 'M', '1986-08-30', '9999-1010', 'Col. Suyapa, Tela, Atlántida', 'Educacion Fisica', 1
FROM Usuario WHERE Usuario = 'daniel.reyes';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Patricia Castro', '0107199500010', 'F', '1995-12-03', '9900-1111', 'Col. San Juan, Tela, Atlántida', 'Artes', 1
FROM Usuario WHERE Usuario = 'patricia.castro';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Roberto Mejia', '0107198800011', 'M', '1988-05-17', '9811-2222', 'Col. La Independencia, Tela, Atlántida', 'Matematicas', 1
FROM Usuario WHERE Usuario = 'roberto.mejia';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Gabriela Navarro', '0107199300012', 'F', '1993-10-09', '9822-3333', 'Barrio Venecia, Tela, Atlántida', 'Biologia', 1
FROM Usuario WHERE Usuario = 'gabriela.navarro';

INSERT INTO Docente (UsuarioID, Nombre, Identidad, Sexo, FechaNacimiento, Telefono, Direccion, Especialidad, Estado)
SELECT UsuarioID, 'Fernando Alvarado', '0107198700013', 'M', '1987-01-28', '9833-4444', 'Col. Suyapa, Tela, Atlántida', 'Ciencias Sociales', 1
FROM Usuario WHERE Usuario = 'fernando.alvarado';







--=========================
-- TABLA TUTOR
--=========================
select * from tutor d
inner join Usuario u on d.UsuarioID = u.UsuarioID

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Carlos Perez', '0107198010001', '9910-1001', 'Padre', 'Comerciante', 1
FROM Usuario U WHERE U.Usuario = 'carlos.perez';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Marta Perez', '0107198210002', '9910-1002', 'Madre', 'Ama de Casa', 1
FROM Usuario U WHERE U.Usuario = 'marta.perez';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Jorge Garcia', '0107197910003', '9910-1003', 'Padre', 'Ingeniero Civil', 1
FROM Usuario U WHERE U.Usuario = 'jorge.garcia';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Silvia Garcia', '0107198310004', '9910-1004', 'Madre', 'Enfermera', 1
FROM Usuario U WHERE U.Usuario = 'silvia.garcia';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Ricardo Ramirez', '0107197810005', '9910-1005', 'Padre', 'Abogado', 1
FROM Usuario U WHERE U.Usuario = 'ricardo.ramirez';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Rosa Fernandez', '0107198510006', '9910-1006', 'Madre', 'Docente', 1
FROM Usuario U WHERE U.Usuario = 'rosa.fernandez';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Oscar Torres', '0107197710007', '9910-1007', 'Padre', 'Contador', 1
FROM Usuario U WHERE U.Usuario = 'oscar.torres';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Veronica Torres', '0107198610008', '9910-1008', 'Madre', 'Administradora', 1
FROM Usuario U WHERE U.Usuario = 'veronica.torres';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Hector Herrera', '0107197510009', '9910-1009', 'Padre', 'Tecnico Electricista', 1
FROM Usuario U WHERE U.Usuario = 'hector.herrera';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Diana Herrera', '0107198810010', '9910-1010', 'Madre', 'Comerciante', 1
FROM Usuario U WHERE U.Usuario = 'diana.herrera';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Luis Morales', '0107197610011', '9910-1011', 'Padre', 'Arquitecto', 1
FROM Usuario U WHERE U.Usuario = 'luis.morales';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Karla Vargas', '0107198910012', '9910-1012', 'Madre', 'Psicologa', 1
FROM Usuario U WHERE U.Usuario = 'karla.vargas';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Sergio Reyes', '0107197410013', '9910-1013', 'Padre', 'Empresario', 1
FROM Usuario U WHERE U.Usuario = 'sergio.reyes';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Andrea Reyes', '0107199010014', '9910-1014', 'Madre', 'Contadora', 1
FROM Usuario U WHERE U.Usuario = 'andrea.reyes';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Miguel Castro', '0107197310015', '9910-1015', 'Padre', 'Mecanico', 1
FROM Usuario U WHERE U.Usuario = 'miguel.castro';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Lucia Castro', '0107199110016', '9910-1016', 'Madre', 'Asistente Administrativa', 1
FROM Usuario U WHERE U.Usuario = 'lucia.castro';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Monica Mejia', '0107199210017', '9910-1017', 'Madre', 'Doctora', 1
FROM Usuario U WHERE U.Usuario = 'monica.mejia';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Rafael Navarro', '0107197210018', '9910-1018', 'Padre', 'Supervisor Industrial', 1
FROM Usuario U WHERE U.Usuario = 'rafael.navarro';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Carlos Alvarado', '0107197010019', '9910-1019', 'Padre', 'Ingeniero Industrial', 1
FROM Usuario U WHERE U.Usuario = 'carlos.alvarado';

INSERT INTO Tutor (UsuarioID, Nombre, Identidad, Telefono, Parentesco, LugarTrabajo, Estado)
SELECT U.UsuarioID, 'Gloria Alvarado', '0107199310020', '9910-1020', 'Madre', 'Licenciada en Educacion', 1
FROM Usuario U WHERE U.Usuario = 'gloria.alvarado';





--=========================
-- TABLA ESTUDIANTES
--=========================
select* from Estudiante
INSERT INTO Estudiante (Nombre, Sexo, Identidad, Direccion, Telefono, FechaNacimiento, Mano, Alergia, Imagen, Estado)
VALUES
-- 1 (9 años → CUARTO grado)
('Luis Perez', 'M', '0101201600011', 'Barrio El Centro, Tela, Atlántida', '9991-0001', '2016-05-14', 'DERECHA', 'Ninguna', 'img/estudiantes/luis_perez.jpg', 1),

-- 2 (7 años → SEGUNDO grado)
('Valeria Garcia', 'F', '0101201800012', 'Col. Trejo, Tela, Atlántida', '9991-0002', '2018-03-22', 'DERECHA', 'Alergia al polvo', 'img/estudiantes/valeria_garcia.jpg', 1),

-- 3 (15 años → DECIMO)
('Diego Ramirez', 'M', '0101201000013', 'Col. San Juan, Tela, Atlántida', '9991-0003', '2010-08-09', 'IZQUIERDA', 'Ninguna', 'img/estudiantes/diego_ramirez.jpg', 1),

-- 4 (6 años → PREPARATORIA)
('Camila Fernandez', 'F', '0101201900014', 'Barrio El Centro, Tela, Atlántida', '9991-0004', '2019-01-18', 'DERECHA', 'Alergia a mariscos', 'img/estudiantes/camila_fernandez.jpg', 1),

-- 5 (13 años → OCTAVO)
('Mateo Torres', 'M', '0101201200015', 'Col. La Esperanza, Tela, Atlántida', '9991-0005', '2012-11-03', 'DERECHA', 'Ninguna', 'img/estudiantes/mateo_torres.jpg', 1),

-- 6 (11 años → SEXTO)
('Sofia Herrera', 'F', '0101201400016', 'Col. Suyapa, Tela, Atlántida', '9991-0006', '2014-07-27', 'AMBIDIESTRO', 'Intolerancia a lactosa', 'img/estudiantes/sofia_herrera.jpg', 1),

-- 7 (16 años → UNDECIMO)
('Gabriel Morales', 'M', '0101200900017', 'Barrio Venecia, Tela, Atlántida', '9991-0007', '2009-04-11', 'DERECHA', 'Ninguna', 'img/estudiantes/gabriel_morales.jpg', 1),

-- 8 (8 años → TERCERO)
('Daniela Vargas', 'F', '0101201700018', 'Col. La Independencia, Tela, Atlántida', '9991-0008', '2017-06-30', 'IZQUIERDA', 'Ninguna', 'img/estudiantes/daniela_vargas.jpg', 1),

-- 9 (14 años → NOVENO)
('Alejandro Reyes', 'M', '0101201100019', 'Barrio El Centro, Tela, Atlántida', '9991-0009', '2011-09-15', 'DERECHA', 'Ninguna', 'img/estudiantes/alejandro_reyes.jpg', 1),

-- 10 (10 años → QUINTO)
('Luciana Castro', 'F', '0101201500020', 'Col. Suyapa, Tela, Atlántida', '9991-0010', '2015-12-05', 'DERECHA', 'Alergia a penicilina', 'img/estudiantes/luciana_castro.jpg', 1),

-- 11 (5 años → KINDER)
('Samuel Mejia', 'M', '0101202000021', 'Col. La Esperanza, Tela, Atlántida', '9991-0011', '2020-02-19', 'DERECHA', 'Ninguna', 'img/estudiantes/samuel_mejia.jpg', 1),

-- 12 (12 años → SEPTIMO)
('Isabella Navarro', 'F', '0101201300022', 'Barrio Venecia, Tela, Atlántida', '9991-0012', '2013-10-08', 'DERECHA', 'Ninguna', 'img/estudiantes/isabella_navarro.jpg', 1),

-- 13 (4 años → PRE-KINDER)
('Thiago Alvarado', 'M', '0101202100023', 'Col. San Juan, Tela, Atlántida', '9991-0013', '2021-06-12', 'AMBIDIESTRO', 'Ninguna', 'img/estudiantes/thiago_alvarado.jpg', 1);




--=========================
-- TABLA TUTORESTUDIANTE
--=========================
select * from TUTORESTUDIANTE
-- 1 Luis Perez
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Carlos Perez','Marta Perez')
AND E.Nombre = 'Luis Perez';


-- 2 Valeria Garcia
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Jorge Garcia','Silvia Garcia')
AND E.Nombre = 'Valeria Garcia';


-- 3 Diego Ramirez
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Ricardo Ramirez'
AND E.Nombre = 'Diego Ramirez';


-- 4 Camila Fernandez
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Rosa Fernandez'
AND E.Nombre = 'Camila Fernandez';


-- 5 Mateo Torres
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Oscar Torres','Veronica Torres')
AND E.Nombre = 'Mateo Torres';


-- 6 Sofia Herrera 
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Hector Herrera','Diana Herrera')
AND E.Nombre = 'Sofia Herrera';


-- 7 Gabriel Morales
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Luis Morales'
AND E.Nombre = 'Gabriel Morales';


-- 8 Daniela Vargas
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Karla Vargas'
AND E.Nombre = 'Daniela Vargas';


-- 9 Alejandro Reyes
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Sergio Reyes','Andrea Reyes')
AND E.Nombre = 'Alejandro Reyes';


-- 10 Luciana Castro
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Miguel Castro','Lucia Castro')
AND E.Nombre = 'Luciana Castro';


-- 11 Samuel Mejia
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Monica Mejia'
AND E.Nombre = 'Samuel Mejia';


-- 12 Isabella Navarro
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre = 'Rafael Navarro'
AND E.Nombre = 'Isabella Navarro';


-- 13 Thiago Alvarado
INSERT INTO TutorEstudiante (TutorID, EstudianteID)
SELECT T.TutorID, E.EstudianteID
FROM Tutor T, Estudiante E
WHERE T.Nombre IN ('Carlos Alvarado','Gloria Alvarado')
AND E.Nombre = 'Thiago Alvarado';





--=========================
-- TABLA ASIGNATURA
--=========================
select * from Asignatura 

INSERT INTO Asignatura (Nombre, Area, Descripcion) VALUES

-- Áreas Curriculares
('Español / Spanish', 'Áreas Curriculares', 'Desarrollo de habilidades de lectura, escritura y comunicación en español'),
('Matemáticas / Math', 'Áreas Curriculares', 'Razonamiento numérico, álgebra y resolución de problemas'),
('Ciencias Naturales / Science', 'Áreas Curriculares', 'Estudio del mundo natural y fenómenos científicos'),
('Ciencias Sociales / Social Studies', 'Áreas Curriculares', 'Historia, geografía y sociedad'),
('Educación Física / Physical Education', 'Áreas Curriculares', 'Actividad física y desarrollo corporal'),
('Educación Cívica', 'Áreas Curriculares', 'Formación ciudadana y valores cívicos'),
('Educación Artística / Arts', 'Áreas Curriculares', 'Expresión creativa y desarrollo artístico'),

-- Áreas de Comunicación Inglés
('Biblia / Bible', 'Áreas de Comunicación Inglés', 'Formación espiritual y valores cristianos en inglés'),
('Historia / History', 'Áreas de Comunicación Inglés', 'Estudio de procesos históricos en inglés'),
('Ortografía / Spelling', 'Áreas de Comunicación Inglés', 'Reglas de escritura en inglés'),
('Literatura / Literature', 'Áreas de Comunicación Inglés', 'Análisis y comprensión de textos literarios en inglés'),
('Gramática / Grammar', 'Áreas de Comunicación Inglés', 'Estructura y reglas del idioma inglés'),
('Fonética / Phonics', 'Áreas de Comunicación Inglés', 'Relación entre sonidos y letras en inglés'),
('Lectura / Reading', 'Áreas de Comunicación Inglés', 'Desarrollo de comprensión lectora en inglés'),

-- Área de Tecnología
('Computación', 'Área de Tecnología', 'Uso de herramientas tecnológicas y fundamentos informáticos'),

-- Personalidad
('Puntualidad', 'Personalidad', 'Evaluación del cumplimiento de horarios'),
('Espíritu de Trabajo', 'Personalidad', 'Compromiso y responsabilidad académica'),
('Orden y Presentación', 'Personalidad', 'Cuidado personal y organización'),
('Sociabilidad', 'Personalidad', 'Capacidad de relación y trabajo en equipo'),
('Moralidad', 'Personalidad', 'Comportamiento ético y respeto'),
('Inasistencia', 'Personalidad', 'Control y seguimiento de ausencias');


--=========================
-- TABLA CARGA ACADEMICA
--=========================
select * from CargaAcademica 
--MATEMATICAS
-- Juan Perez (Primaria)
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Juan Perez'
AND A.Nombre = 'Matemáticas / Math'
AND S.SeccionID IN (7,9,11,13,15,17);

-- Roberto Mejia (Secundaria)
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Roberto Mejia'
AND A.Nombre = 'Matemáticas / Math'
AND S.SeccionID IN (19,21,23,25,27);


--Español
--Ana Garcia (Primaria)
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Ana Garcia'
AND A.Nombre = 'Español / Spanish'
AND S.SeccionID IN (7,9,11,13,15,17);

--Luisa Fernandez (Secundaria)
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Luisa Fernandez'
AND A.Nombre = 'Español / Spanish'
AND S.SeccionID IN (19,21,23,25,27);


--CIVICA
--Luisa Fernandez (Secundaria)
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Luisa Fernandez'
AND A.Nombre = 'Educación Cívica'
AND S.SeccionID IN (19,21,23,25,27);



--CIENCIAS NATURALES
--Carlos Ramirez 
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Carlos Ramirez'
AND A.Nombre = 'Ciencias Naturales / Science'
AND S.SeccionID IN (7,9,11,13,15,17);


-- Gabriela Navarro
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Gabriela Navarro'
AND A.Nombre = 'Ciencias Naturales / Science'
AND S.SeccionID IN (19,21,23,25,27);


--CIENCIAS SOCIALES
--Fernando Alvarado
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Fernando Alvarado'
AND A.Nombre = 'Ciencias Sociales / Social Studies'
AND S.SeccionID IN (7,9,11,13,15,17);

--Miguel Torres
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Miguel Torres'
AND A.Nombre = 'Ciencias Sociales / Social Studies'
AND S.SeccionID IN (19,21,23,25,27);




--COMPUTACION
--Andres Morales
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Andres Morales'
AND A.Nombre = 'Computación'
AND S.SeccionID IN (7,9,11,13,15,17,19,21,23,25,27);


--AREAS DE COMUNICACION DE INGLES
--Elena Vargas
--Biblia / Bible, Ortograf�a / Spelling, Gram�tica / Grammar,Fon�tica / Phonics,Lectura / Reading
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Elena Vargas'
AND A.Area = 'Áreas de Comunicación Inglés'
AND S.SeccionID IN (7,9,11,13,15,17);


-- Sofia Herrera
--Biblia / Bible, Ortograf�a / Spelling,Literatura / Literature,Gram�tica / Grammar
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Sofia Herrera'
AND A.Area = 'Áreas de Comunicación Inglés'
AND S.SeccionID IN (19,21,23,25,27);


--HISTORIA/ HISTORY
--Miguel Torres
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Miguel Torres'
AND A.Nombre = 'Historia / History'
AND S.SeccionID IN (19,21,23,25,27);


--EDUCACION FISICA
--Daniel Reyes
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Daniel Reyes'
AND A.Nombre = 'Educación Física / Physical Education';


---Educaci�n Art�stica / Arts
--Patricia Castro
INSERT INTO CargaAcademica (DocenteID, AsignaturaID, SeccionID, Estado)
SELECT D.DocenteID, A.AsignaturaID, S.SeccionID, 1
FROM Docente D, Asignatura A, Seccion S
WHERE D.Nombre = 'Patricia Castro'
AND A.Nombre = 'Educación Artística / Arts'




--=========================
-- TABLA MATRICULA
--=========================
select * from Matricula


-- 1 Luis Perez → CUARTO (SeccionID 13)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 13, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Luis Perez';


-- 2 Valeria Garcia → SEGUNDO (9)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 9, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Valeria Garcia';


-- 3 Diego Ramirez → DECIMO (25)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 25, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Diego Ramirez';


-- 4 Camila Fernandez → PREPARATORIA (5)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 5, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Camila Fernandez';


-- 5 Mateo Torres → OCTAVO (21)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 21, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Mateo Torres';


-- 6 Sofia Herrera → SEXTO (17)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 17, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Sofia Herrera';


-- 7 Gabriel Morales → UNDECIMO (27)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 27, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Gabriel Morales';


-- 8 Daniela Vargas → TERCERO (11)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 11, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Daniela Vargas';


-- 9 Alejandro Reyes → NOVENO (23)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 23, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Alejandro Reyes';


-- 10 Luciana Castro → QUINTO (15)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 15, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Luciana Castro';


-- 11 Samuel Mejia → KINDER (3)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 3, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Samuel Mejia';


-- 12 Isabella Navarro → SEPTIMO (19)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 19, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Isabella Navarro';


-- 13 Thiago Alvarado → PRE-KINDER (1)
INSERT INTO Matricula (EstudianteID, SeccionID, Fecha, Anio)
SELECT EstudianteID, 1, '2026-01-15', 2026
FROM Estudiante WHERE Nombre = 'Thiago Alvarado';







--=========================
-- TABLA ASISTENCIA
--=========================

--INSERT INTO Asistencia (EstudianteID, CargaID, Fecha, Estado, Observacion)
SELECT 
    M.EstudianteID,
    CA.CargaID,
    F.Fecha,
    CASE 
        WHEN RAND(CHECKSUM(NEWID())) < 0.80 THEN 'PRESENTE'
        WHEN RAND(CHECKSUM(NEWID())) < 0.90 THEN 'TARDE'
        WHEN RAND(CHECKSUM(NEWID())) < 0.97 THEN 'JUSTIFICADO'
        ELSE 'AUSENTE'
    END AS Estado,
    CASE 
        WHEN RAND(CHECKSUM(NEWID())) < 0.05 THEN 'Enfermedad'
        WHEN RAND(CHECKSUM(NEWID())) < 0.03 THEN 'Cita médica'
        ELSE NULL
    END AS Observacion
FROM Matricula M
JOIN CargaAcademica CA 
    ON CA.SeccionID = M.SeccionID
CROSS JOIN (
    SELECT CAST('2026-03-01' AS DATE) AS Fecha
    UNION ALL SELECT '2026-03-02'
    UNION ALL SELECT '2026-03-03'
    UNION ALL SELECT '2026-03-04'
    UNION ALL SELECT '2026-03-05'
) F
WHERE DATENAME(WEEKDAY, F.Fecha) NOT IN ('Saturday','Sunday')





--=========================
-- TABLA ACTIVIDAD
--=========================


INSERT INTO Actividad (CargaID, Descripcion, Parcial, Valor)
SELECT 
    CA.CargaID,
    CONCAT(T.Tipo, ' - ', A.Nombre) AS Descripcion,
    1,
    T.Valor
FROM CargaAcademica CA
JOIN Asignatura A ON A.AsignaturaID = CA.AsignaturaID
CROSS JOIN (
    -- 3 TAREAS
    SELECT 'Tarea 1' AS Tipo, 1 AS Parcial, 10 AS Valor UNION ALL
    SELECT 'Tarea 2', 1, 10 UNION ALL
    SELECT 'Tarea 3', 2, 10 UNION ALL

    -- PRUEBA
    SELECT 'Prueba', 3, 30 UNION ALL

    -- EXAMEN
    SELECT 'Examen', 4, 40
) T
WHERE CA.SeccionID IN (SELECT SeccionID FROM Seccion WHERE Letra = 'A')



SELECT 
    G.NombreGrado,
    S.Letra AS Seccion,
    A.Nombre AS Asignatura,
    D.Nombre AS Docente,
    AC.Descripcion,
    AC.Parcial,
    AC.Valor
FROM Actividad AC
JOIN CargaAcademica CA ON CA.CargaID = AC.CargaID
JOIN Asignatura A ON A.AsignaturaID = CA.AsignaturaID
JOIN Docente D ON D.DocenteID = CA.DocenteID
JOIN Seccion S ON S.SeccionID = CA.SeccionID
JOIN Grado G ON G.GradoID = S.GradoID
ORDER BY 
    G.GradoID,
    A.Nombre,
    AC.Parcial;


    
--=========================
-- TABLA CALIFICACION
--=========================


INSERT INTO Calificacion (EstudianteID, ActividadID, Nota, Fecha)
SELECT 
    M.EstudianteID,
    AC.ActividadID,
    (AC.Valor * 0.6) + (ABS(CHECKSUM(NEWID())) % CAST((AC.Valor * 0.4) + 1 AS INT)) AS Nota,
    GETDATE()
FROM Matricula M
JOIN CargaAcademica CA 
    ON CA.SeccionID = M.SeccionID
JOIN Actividad AC 
    ON AC.CargaID = CA.CargaID
WHERE AC.Descripcion LIKE 'Tarea%'


SELECT 
    E.EstudianteID,
    E.Nombre AS Estudiante,
    A.Nombre AS Asignatura,
    AC.Descripcion AS Actividad,
    AC.Parcial,
    AC.Valor,
    C.Nota,
    C.Fecha
FROM Calificacion C
JOIN Estudiante E ON E.EstudianteID = C.EstudianteID
JOIN Actividad AC ON AC.ActividadID = C.ActividadID
JOIN CargaAcademica CA ON CA.CargaID = AC.CargaID
JOIN Asignatura A ON A.AsignaturaID = CA.AsignaturaID
ORDER BY 
    E.Nombre,
    A.Nombre,
    AC.Parcial;






--=========================
-- TABLA REUNIONES
--=========================
select * from Reunion where Estado = 'REALIZADA'
-- 1 Juan Perez - Luis Perez
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-01 08:00:00',
       'Rendimiento en Matemáticas', 'PRESENCIAL', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Luis Perez'
WHERE D.Nombre = 'Juan Perez';


-- 2 Ana Garcia - Valeria Garcia
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-02 09:30:00',
       'Dificultades en Lectura', 'VIDEOLLAMADA', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Valeria Garcia'
WHERE D.Nombre = 'Ana Garcia';


-- 3 Sofia Herrera - Diego Ramirez
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-03 10:00:00',
       'Bajo desempeño en Grammar', 'PRESENCIAL', 'PROGRAMADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Diego Ramirez'
WHERE D.Nombre = 'Sofia Herrera';


-- 4 Miguel Torres - Camila Fernandez
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-04 11:00:00',
       'Conducta en clase', 'LLAMADA', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Camila Fernandez'
WHERE D.Nombre = 'Miguel Torres';


-- 5 Andres Morales - Mateo Torres
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-05 08:30:00',
       'Uso adecuado de tecnología', 'VIDEOLLAMADA', 'PROGRAMADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Mateo Torres'
WHERE D.Nombre = 'Andres Morales';


-- 6 Elena Vargas - Sofia Herrera
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-06 09:00:00',
       'Progreso en Inglés', 'PRESENCIAL', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Sofia Herrera'
WHERE D.Nombre = 'Elena Vargas';


-- 7 Daniel Reyes - Gabriel Morales
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-07 10:30:00',
       'Participación en Educación Física', 'PRESENCIAL', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Gabriel Morales'
WHERE D.Nombre = 'Daniel Reyes';


-- 8 Patricia Castro - Daniela Vargas
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-08 08:45:00',
       'Desarrollo artístico', 'VIDEOLLAMADA', 'CANCELADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Daniela Vargas'
WHERE D.Nombre = 'Patricia Castro';


-- 9 Roberto Mejia - Alejandro Reyes
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-09 11:15:00',
       'Refuerzo en Matemáticas', 'LLAMADA', 'REALIZADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Alejandro Reyes'
WHERE D.Nombre = 'Roberto Mejia';


-- 10 Fernando Alvarado - Luciana Castro
INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
SELECT D.DocenteID, E.EstudianteID, '2026-03-10 09:20:00',
       'Seguimiento en Ciencias Sociales', 'PRESENCIAL', 'CANCELADA'
FROM Docente D
INNER JOIN Estudiante E ON E.Nombre = 'Luciana Castro'
WHERE D.Nombre = 'Fernando Alvarado';




--=========================
-- TABLA ACTAS
--=========================
select * from Acta

-- 1 Rendimiento en Matemáticas
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-01',
'Se acordó reforzar ejercicios prácticos y establecer tutorías semanales.',
'Padres comprometidos a supervisar tareas en casa.'
FROM Reunion R
WHERE R.Tema = 'Rendimiento en Matemáticas'
AND R.Estado = 'REALIZADA';


-- 2 Dificultades en Lectura
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-02',
'Lectura diaria obligatoria y seguimiento semanal del progreso.',
'Se recomienda apoyo constante en casa.'
FROM Reunion R
WHERE R.Tema = 'Dificultades en Lectura'
AND R.Estado = 'REALIZADA';


-- 3 Conducta en clase
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-04',
'Implementar plan de mejora conductual y seguimiento mensual.',
'Se observará evolución en las próximas semanas.'
FROM Reunion R
WHERE R.Tema = 'Conducta en clase'
AND R.Estado = 'REALIZADA';


-- 4 Progreso en Inglés
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-06',
'Asignación de prácticas adicionales y participación en actividades orales.',
'Buen avance general.'
FROM Reunion R
WHERE R.Tema = 'Progreso en Inglés'
AND R.Estado = 'REALIZADA';


-- 5 Participación en Educación Física
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-07',
'Motivar participación en actividades deportivas extracurriculares.',
'Condición física adecuada.'
FROM Reunion R
WHERE R.Tema = 'Participación en Educación Física'
AND R.Estado = 'REALIZADA';


-- 6 Refuerzo en Matemáticas
INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
SELECT R.ReunionID, '2026-03-09',
'Clases de refuerzo dos veces por semana.',
'Necesita mayor práctica constante.'
FROM Reunion R
WHERE R.Tema = 'Refuerzo en Matemáticas'
AND R.Estado = 'REALIZADA';








--=========================
-- TABLA Boleta
--=========================

INSERT INTO Boleta (EstudianteID, DocenteID, Anio, PromedioGeneral)
SELECT 
    E.EstudianteID,

    -- Tomamos un docente del estudiante (puede ser cualquiera de sus clases)
    MIN(CA.DocenteID) AS DocenteID,

    2026 AS Anio,

    -- Promedio general ponderado
    CAST(
        (SUM(C.Nota) * 100.0 / SUM(AC.Valor)) 
        AS DECIMAL(5,2)
    ) AS PromedioGeneral

FROM Estudiante E
JOIN Matricula M 
    ON M.EstudianteID = E.EstudianteID
JOIN CargaAcademica CA 
    ON CA.SeccionID = M.SeccionID
JOIN Actividad AC 
    ON AC.CargaID = CA.CargaID
JOIN Calificacion C 
    ON C.ActividadID = AC.ActividadID
   AND C.EstudianteID = E.EstudianteID

GROUP BY 
    E.EstudianteID;


--=========================
-- TABLA Boleta Detalle
--=========================


INSERT INTO BoletaDetalle (BoletaID, AsignaturaID, NotaP1, NotaP2, NotaP3, NotaP4, Recuperacion)
SELECT 
    B.BoletaID,
    A.AsignaturaID,

    -- PARCIAL 1
    CAST(
        SUM(CASE WHEN AC.Parcial = 1 THEN C.Nota ELSE 0 END) * 100.0 /
        NULLIF(SUM(CASE WHEN AC.Parcial = 1 THEN AC.Valor ELSE 0 END),0)
    AS DECIMAL(5,2)) AS NotaP1,

    -- PARCIAL 2
    CAST(
        SUM(CASE WHEN AC.Parcial = 2 THEN C.Nota ELSE 0 END) * 100.0 /
        NULLIF(SUM(CASE WHEN AC.Parcial = 2 THEN AC.Valor ELSE 0 END),0)
    AS DECIMAL(5,2)) AS NotaP2,

    -- PARCIAL 3
    CAST(
        SUM(CASE WHEN AC.Parcial = 3 THEN C.Nota ELSE 0 END) * 100.0 /
        NULLIF(SUM(CASE WHEN AC.Parcial = 3 THEN AC.Valor ELSE 0 END),0)
    AS DECIMAL(5,2)) AS NotaP3,

    -- PARCIAL 4
    CAST(
        SUM(CASE WHEN AC.Parcial = 4 THEN C.Nota ELSE 0 END) * 100.0 /
        NULLIF(SUM(CASE WHEN AC.Parcial = 4 THEN AC.Valor ELSE 0 END),0)
    AS DECIMAL(5,2)) AS NotaP4,

    -- RECUPERACION (NULL por ahora)
    NULL

FROM Boleta B
JOIN Estudiante E 
    ON E.EstudianteID = B.EstudianteID
JOIN Matricula M 
    ON M.EstudianteID = E.EstudianteID
JOIN CargaAcademica CA 
    ON CA.SeccionID = M.SeccionID
JOIN Asignatura A 
    ON A.AsignaturaID = CA.AsignaturaID
JOIN Actividad AC 
    ON AC.CargaID = CA.CargaID
JOIN Calificacion C 
    ON C.ActividadID = AC.ActividadID
   AND C.EstudianteID = E.EstudianteID

WHERE B.Anio = 2026

GROUP BY 
    B.BoletaID,
    A.AsignaturaID;



SELECT 
    E.EstudianteID,
    E.Nombre AS Estudiante,
    B.Anio,
    B.PromedioGeneral,

    A.Nombre AS Asignatura,
    BD.NotaP1,
    BD.NotaP2,
    BD.NotaP3,
    BD.NotaP4,
    BD.Recuperacion

FROM Boleta B
JOIN Estudiante E 
    ON E.EstudianteID = B.EstudianteID
JOIN BoletaDetalle BD 
    ON BD.BoletaID = B.BoletaID
JOIN Asignatura A 
    ON A.AsignaturaID = BD.AsignaturaID

ORDER BY 
    E.Nombre,
    A.Nombre;

















