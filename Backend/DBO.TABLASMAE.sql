

-- USE AGROLINKDB


-- TABLAS DE MAE DB

-- TABLA CONFIGURACION

CREATE TABLE Configuracion
(
    -- llave primaria autoincrementable
    ConfigID INT IDENTITY(1,1) NOT NULL,

    -- configuracion academica
    Anio INT NOT NULL,
    Periodo INT NOT NULL,

    -- 1 ? docentes pueden ingresar notas y 0 ? sistema bloquea ingreso
    -- SubirNotas BIT NOT NULL
    -- CONSTRAINT dfConfiguracionSubirNotas DEFAULT 0,

    -- cambie SubirNotas por Activa para indicar si es la configuracion actualmente vigente
    Activa BIT NOT NULL
    CONSTRAINT dfConfiguracionActiva DEFAULT 1,

    -- definicion de llaves y condiciones
    CONSTRAINT pkConfiguracion PRIMARY KEY (ConfigID),

    -- evita duplicar configuracion del mismo a�o y periodo
    CONSTRAINT uqConfiguracionAnioPeriodo UNIQUE (Anio, Periodo),

    -- validacion de periodo (4 periodos)
    CONSTRAINT ckConfiguracionPeriodo CHECK (Periodo BETWEEN 1 AND 4)
)


-- TABLA USUARIO

CREATE TABLE Usuario 
(
    -- llave primaria autoincrementable
    UsuarioID INT IDENTITY(1,1) NOT NULL, 
    
    -- credenciales de acceso
    Usuario VARCHAR(50) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    Password VARCHAR(255) NOT NULL, -- longitud amplia para contrasenias encripted
    
    -- clasificacion del usuario (Ej: Admin, Docente, Tutor)
    Rol VARCHAR(20) NOT NULL,
    
    -- control de estado
    Estado BIT NOT NULL
    CONSTRAINT dfUsuarioEstado DEFAULT 1, 

    -- definicion de llaves y condiciones
    CONSTRAINT pkUsuarioID PRIMARY KEY (UsuarioID),

    CONSTRAINT uqUsuarioNombre UNIQUE (Usuario),
    CONSTRAINT uqUsuarioCorreo UNIQUE (Correo),

    CONSTRAINT ckUsuarioRol CHECK (Rol IN ('Administrador', 'Docente', 'Tutor'))
)


-- TABLA ADMINISTRADOR

CREATE TABLE Admin (
    -- llave primaria autoincrementable
    AdminID INT IDENTITY(1,1) NOT NULL, 
    
    -- relacion con la cuenta de acceso
    UsuarioID INT NOT NULL, 
    
    -- datos personales y laborales
    Nombre VARCHAR(100) NOT NULL,
    Identidad VARCHAR(20) NOT NULL,
    Sexo CHAR(1) NOT NULL, -- restriccion para Masculino/Femenino
    Telefono VARCHAR(20),
    Direccion VARCHAR(255),
    
    Posicion VARCHAR(50), 
    
    -- control de acceso al sistema (1=Activo, 0=Inactivo)
    Estado BIT NOT NULL, 

    -- definicion de la llaves y condiciones
    CONSTRAINT pkAdminID PRIMARY KEY (AdminID),
    CONSTRAINT fkAdminUsuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),

    CONSTRAINT uqAdminIdentidad UNIQUE (Identidad),

    CONSTRAINT ckAdminSexo CHECK (Sexo in ('M','F'))
)



-- TABLA DOCENTE

CREATE TABLE Docente 
(
    -- llave primaria autoincrementable
    DocenteID INT IDENTITY(1,1) NOT NULL, 
    
    -- relacion con la tabla de credenciales
    UsuarioID INT NOT NULL, 
    
    -- informacion personal
    Nombre VARCHAR(100) NOT NULL,
    Identidad VARCHAR(20) NOT NULL, -- unique para que no se repita
    Sexo CHAR(1) NOT NULL, -- restriccion para Masculino/Femenino
    FechaNacimiento DATE NOT NULL,
    
    -- contacto y perfil
    Telefono VARCHAR(20),
    Direccion VARCHAR(255),
    Especialidad VARCHAR(100),
    
    -- control administrativo
    Estado BIT NOT NULL
    CONSTRAINT dfDocenteEstado default 1, -- 1 para Activo, 0 para Inactivo

    -- definicion de la llaves y condiciones
    CONSTRAINT pkDocenteID PRIMARY KEY (DocenteID),
    CONSTRAINT fkDocenteUsuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),

    CONSTRAINT uqDocenteIdentidad UNIQUE (Identidad),

    CONSTRAINT ckDocenteSexo CHECK (Sexo in ('M','F'))
)


-- TABLA TUTOR

CREATE TABLE Tutor
(
    -- llave primaria autoincrementable
    TutorID INT IDENTITY(1,1) NOT NULL,

    -- relacion con usuario (credenciales)
    UsuarioID INT NOT NULL,

    -- informacion personal
    Nombre VARCHAR(100) NOT NULL,
    Identidad VARCHAR(20) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Parentesco VARCHAR(50) NOT NULL,
    Lugartrabajo VARCHAR(150),

    -- control administrativo
    Estado BIT NOT NULL
    CONSTRAINT dfTutorEstado DEFAULT 1,

    -- definicion de llaves y condiciones
    CONSTRAINT pkTutorID PRIMARY KEY (tutorid),
    CONSTRAINT fkTutorUsuario FOREIGN KEY (usuarioid) REFERENCES usuario(usuarioid),

    CONSTRAINT uqTutorIdentidad UNIQUE (identidad)
)


-- TABLA ESTUDIANTE

CREATE TABLE Estudiante
(
    -- llave primaria autoincrementable
    EstudianteID INT IDENTITY(1,1) NOT NULL,

    -- informacion personal
    Nombre VARCHAR(100) NOT NULL,
    Sexo CHAR(1) NOT NULL,
    Identidad VARCHAR(20) NOT NULL,
    Direccion VARCHAR(255),
    Telefono VARCHAR(20),
    FechaNacimiento DATE NOT NULL,

    -- informacion adicional
    Mano VARCHAR(15), 
    Alergia VARCHAR(255),
    Imagen VARCHAR(255), -- guardamos solo la ruta de la imagen y se almacena en el servidor, no en la BD

    -- control administrativo
    Estado BIT NOT NULL 
    CONSTRAINT dfEstudianteEstado DEFAULT 1,

    -- definicion de llaves y restricciones
    CONSTRAINT pkEstudianteID PRIMARY KEY (EstudianteID),

    CONSTRAINT uqEstudianteIdentidad UNIQUE (Identidad),

    CONSTRAINT ckEstudianteSexo CHECK (Sexo IN ('M','F')),
    CONSTRAINT ckEstudianteMano CHECK (Mano IN ('DERECHA','IZQUIERDA','AMBIDIESTRO'))
)


-- TABLA TUTOR-ESTUDIANTE

CREATE TABLE TutorEstudiante
(
    -- relacion entre tutor y estudiante
    TutorID INT NOT NULL,
    EstudianteID INT NOT NULL,

    -- llave primaria compuesta
    CONSTRAINT pkTutorEstudiante PRIMARY KEY (TutorID, EstudianteID),

    -- llaves foraneas
    CONSTRAINT fkTutorEstudianteTutor FOREIGN KEY (TutorID) REFERENCES Tutor(TutorID),
    CONSTRAINT fkTutorEstudianteEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID)
)


-- TABLA ASIGNATURA

CREATE TABLE Asignatura
(
    -- llave primaria autoincrementable
    AsignaturaID INT IDENTITY(1,1) NOT NULL,

    -- informacion general
    Nombre VARCHAR(100) NOT NULL,
    Area VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255),

    -- definicion de llaves y condiciones
    CONSTRAINT pkAsignatura PRIMARY KEY (AsignaturaID),

    -- evita nombres de asignaturas duplicados
    CONSTRAINT uqAsignaturaNombre UNIQUE (Nombre)
)


-- TABLA GRADO

CREATE TABLE Grado
(
    -- llave primaria autoincrementable
    GradoID INT IDENTITY(1,1) NOT NULL,

    -- informacion academica
    NombreGrado VARCHAR(100) NOT NULL,
    Nivel VARCHAR(50) NOT NULL,

    -- control administrativo
    Estado BIT NOT NULL
    CONSTRAINT dfGradoEstado DEFAULT 1,

    -- definicion de llaves y condiciones
    CONSTRAINT pkGrado PRIMARY KEY (GradoID),

    -- evita grados duplicados en el mismo nivel
    CONSTRAINT uqGradoNombreNivel UNIQUE (NombreGrado, Nivel)
)


-- TABLA SECCION

CREATE TABLE Seccion
(
    -- llave primaria autoincrementable
    SeccionID INT IDENTITY(1,1) NOT NULL,

    -- relacion con grado
    GradoID INT NOT NULL,

    -- informacion academica
    Letra CHAR(1) NOT NULL,
    Turno VARCHAR(20) NOT NULL,
    Aula VARCHAR(20),

    -- definicion de llaves y condiciones
    CONSTRAINT pkSeccion PRIMARY KEY (SeccionID),

    -- evita duplicar la misma seccion en el mismo anio
    CONSTRAINT uqSeccionGradoLetraAnio UNIQUE (GradoID, Letra, Anio),

    -- validacion de turno
    CONSTRAINT ckSeccionTurno CHECK (Turno IN ('MATUTINO','VESPERTINO','NOCTURNO'))
)


-- TABLA CARGA-ACADEMICA

CREATE TABLE CargaAcademica
(
    -- llave primaria autoincrementable
    CargaID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    DocenteID INT NOT NULL,
    AsignaturaID INT NOT NULL,
    SeccionID INT NOT NULL,
    Anio INT NOT NULL,

    -- control administrativo
    Estado BIT NOT NULL
    CONSTRAINT dfCargaAcademicaEstado DEFAULT 1,

    -- definicion de llaves y condiciones
    CONSTRAINT pkCargaAcademica PRIMARY KEY (CargaID),
    CONSTRAINT fkCargaAcademicaDocente FOREIGN KEY (DocenteID) REFERENCES Docente(DocenteID),
    CONSTRAINT fkCargaAcademicaAsignatura FOREIGN KEY (AsignaturaID) REFERENCES Asignatura(AsignaturaID),
    CONSTRAINT fkCargaAcademicaSeccion FOREIGN KEY (SeccionID) REFERENCES Seccion(SeccionID),

    -- evita que se repita la misma combinacion docente-asignatura-seccion
    CONSTRAINT uqCargaAcademica UNIQUE (DocenteID, AsignaturaID, SeccionID, Anio)
)    


-- TABLA ACTIVIDAD

CREATE TABLE Actividad
(
    -- llave primaria autoincrementable
    ActividadID INT IDENTITY(1,1) NOT NULL,

    -- relacion con la carga academica
    CargaID INT NOT NULL,

    -- informacion de la actividad
    Descripcion VARCHAR(255) NOT NULL,
    Parcial INT NOT NULL,
    Valor DECIMAL(5,2) NOT NULL,

    -- definicion de llaves y condiciones
    CONSTRAINT pkActividad PRIMARY KEY (ActividadID),
    CONSTRAINT fkActividadCarga FOREIGN KEY (CargaID) REFERENCES CargaAcademica(CargaID),

    CONSTRAINT ckActividadParcial CHECK (Parcial BETWEEN 1 AND 4),
    CONSTRAINT ckActividadValor CHECK (Valor BETWEEN 0 AND 100)
)


-- TABLA CALIFICACION

CREATE TABLE Calificacion
(
    -- llave primaria autoincrementable
    CalificacionID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    EstudianteID INT NOT NULL,
    ActividadID INT NOT NULL,

    -- informacion de evaluacion
    Nota DECIMAL(5,2) NOT NULL,
    Fecha DATE NOT NULL,

    -- definicion de llaves y condiciones
    CONSTRAINT pkCalificacion PRIMARY KEY (CalificacionID),

    CONSTRAINT fkCalificacionEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID),
    CONSTRAINT fkCalificacionActividad FOREIGN KEY (ActividadID) REFERENCES Actividad(ActividadID),

    -- evita duplicar nota para el mismo estudiante en la misma actividad
    CONSTRAINT uqCalificacionEstudianteActividad UNIQUE (EstudianteID, ActividadID),

    -- validacion de rango de nota
    CONSTRAINT ckCalificacionNota CHECK (Nota BETWEEN 0 AND 100),
)


-- TABLE ASISTENCIA

CREATE TABLE Asistencia
(
    -- llave primaria autoincrementable
    AsistenciaID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    EstudianteID INT NOT NULL,
    CargaID INT NOT NULL,

    -- informacion de asistencia
    Fecha DATE NOT NULL,
    Estado VARCHAR(15) NOT NULL,
    Observacion VARCHAR(255),

    -- definicion de llaves y condiciones
    CONSTRAINT pkAsistencia PRIMARY KEY (AsistenciaID),
    CONSTRAINT fkAsistenciaEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID),
    CONSTRAINT fkAsistenciaCarga FOREIGN KEY (CargaID) REFERENCES CargaAcademica(CargaID),

    -- evita duplicar asistencia del mismo estudiante en la misma clase y fecha
    CONSTRAINT uqAsistenciaEstudianteCargaFecha UNIQUE (EstudianteID, CargaID, Fecha),

    -- validacion de estado de asistencia
    CONSTRAINT ckAsistenciaEstado CHECK (Estado IN ('PRESENTE','AUSENTE','JUSTIFICADO','TARDE'))
)


-- TABLA REUNION

CREATE TABLE Reunion
(
    -- llave primaria autoincrementable
    ReunionID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    DocenteID INT NOT NULL,
    EstudianteID INT NOT NULL,

    -- informacion de la reunion
    FechaHora DATETIME NOT NULL,
    Tema VARCHAR(255) NOT NULL,
    MedioDifusion VARCHAR(50) NOT NULL,

    -- control administrativo
    Estado VARCHAR(20) NOT NULL,

    -- definicion de llaves y condiciones
    CONSTRAINT pkReunion PRIMARY KEY (ReunionID),
    CONSTRAINT fkReunionDocente FOREIGN KEY (DocenteID) REFERENCES Docente(DocenteID),
    CONSTRAINT fkReunionEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID),

    -- validacion de medio de difusion
    CONSTRAINT ckReunionMedio CHECK (MedioDifusion IN ('PRESENCIAL','VIDEOLLAMADA','LLAMADA')),

    -- validacion de estado
    CONSTRAINT ckReunionEstado CHECK (Estado IN ('PROGRAMADA','REALIZADA','CANCELADA'))
)


-- TABLA ACTA

CREATE TABLE Acta
(
    -- llave primaria autoincrementable
    ActaID INT IDENTITY(1,1) NOT NULL,

    -- relacion con la reunion
    ReunionID INT NOT NULL,

    -- informacion del acta
    Fecha DATE NOT NULL,
    Acuerdos VARCHAR(MAX) NOT NULL,
    Observaciones VARCHAR(255),

    -- definicion de llaves y condiciones
    CONSTRAINT pkActa PRIMARY KEY (ActaID),
    CONSTRAINT fkActaReunion FOREIGN KEY (ReunionID) REFERENCES Reunion(ReunionID),

    -- evita multiples actas para la misma reunion
    CONSTRAINT uqActaReunion UNIQUE (ReunionID)
)


-- TABLA MATRICULA

CREATE TABLE Matricula
(
    -- llave primaria autoincrementable
    MatriculaID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    EstudianteID INT NOT NULL,
    SeccionID INT NOT NULL,

    -- informacion academica
    Fecha DATE NOT NULL,
    Anio INT NOT NULL,

    -- definicion de llaves y condiciones
    CONSTRAINT pkMatricula PRIMARY KEY (MatriculaID),
    CONSTRAINT fkMatriculaEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID),
    CONSTRAINT fkMatriculaSeccion FOREIGN KEY (SeccionID) REFERENCES Seccion(SeccionID),

    -- evita que un estudiante se matricule dos veces en el mismo a�o
    CONSTRAINT uqMatriculaEstudianteAnio UNIQUE (EstudianteID, Anio)
)


-- TABLA BOLETA

CREATE TABLE Boleta
(
    -- llave primaria autoincrementable
    BoletaID INT IDENTITY(1,1) NOT NULL,

    -- relaciones principales
    EstudianteID INT NOT NULL,
    DocenteID INT NOT NULL,

    -- informacion academica
    Anio INT NOT NULL,
    PromedioGeneral DECIMAL(5,2),

    -- control administrativo
    Estado BIT NOT NULL 
    CONSTRAINT dfBoletaEstado DEFAULT 1,

    -- definicion de llaves
    CONSTRAINT pkBoleta PRIMARY KEY (BoletaID),
    CONSTRAINT fkBoletaEstudiante FOREIGN KEY (EstudianteID) REFERENCES Estudiante(EstudianteID),
    CONSTRAINT fkBoletaDocente FOREIGN KEY (DocenteID) REFERENCES Docente(DocenteID),

    -- evita duplicar boletas del mismo estudiante en el mismo a�o
    CONSTRAINT uqBoletaEstudianteAnio UNIQUE (EstudianteID, Anio)
)


-- BOLETA DETALLE

CREATE TABLE BoletaDetalle
(
    -- relaciones principales
    BoletaID INT NOT NULL,
    AsignaturaID INT NOT NULL,

    -- notas x periodo
    NotaP1 DECIMAL(5,2),
    NotaP2 DECIMAL(5,2),
    NotaP3 DECIMAL(5,2),
    NotaP4 DECIMAL(5,2),
    Recuperacion DECIMAL(5,2),

    -- llaves y condiciones
    CONSTRAINT pkBoletaDetalle PRIMARY KEY (BoletaID, AsignaturaID),
    CONSTRAINT fkBoletaDetalleBoleta FOREIGN KEY (BoletaID) REFERENCES Boleta(BoletaID),
    CONSTRAINT fkBoletaDetalleAsignatura FOREIGN KEY (AsignaturaID) REFERENCES Asignatura(AsignaturaID),

    CONSTRAINT ckBoletaDetalleNotasP1 CHECK (NotaP1 BETWEEN 0 AND 100),
    CONSTRAINT ckBoletaDetalleNotasP2 CHECK (NotaP2 BETWEEN 0 AND 100),
    CONSTRAINT ckBoletaDetalleNotasP3 CHECK (NotaP3 BETWEEN 0 AND 100),
    CONSTRAINT ckBoletaDetalleNotasP4 CHECK (NotaP4 BETWEEN 0 AND 100),
    CONSTRAINT ckBoletaDetalleRecuperacion CHECK (Recuperacion BETWEEN 0 AND 100)
)
