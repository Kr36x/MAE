use AgroLinkDB

select * from Asistencia
select * from Estudiante
select * from CargaAcademica
select * from Asignatura

go

create or alter procedure sp_Ingresar_Asistencias 
-- Datos a Ingresar
@grado varchar(100),@seccion varchar(50),@fecha date,@Asignatura varchar(20),
@Estado varchar(50),@observacion varchar(50)=NULL,@Estudiante varchar(100)
as
BEGIN
 begin transaction
 declare @err int = 0,  @duplicado int = 0,@EstudianteID int,@CargaID int;

 --por si las dudas
 IF @Estado NOT IN ('PRESENTE', 'AUSENTE', 'JUSTIFICADO', 'TARDE')
        BEGIN
            RAISERROR('Solo se permite PRESENTE, AUSENTE, JUSTIFICADO o TARDE.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

-- buscamos carga ID 
select @CargaID=ca.CargaID from CargaAcademica CA inner join Asignatura A on CA.AsignaturaID=A.AsignaturaID where a.Nombre=@Asignatura

select @EstudianteID=E.EstudianteID from Estudiante E where Nombre=@Estudiante


insert into Asistencia (EstudianteID,CargaID,Fecha,Estado,Observacion)
values (@EstudianteID,@CargaID,@fecha,@Estado,@observacion)
		
COMMIT TRANSACTION;

 end;

go
