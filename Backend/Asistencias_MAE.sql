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

select @duplicado=count(*) from Asistencia where EstudianteID=@EstudianteID and Fecha=@fecha

IF @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 500010, 'ERROR: Ya se tomo asistencia de este usuario.', 1;
		END; 


insert into Asistencia (EstudianteID,CargaID,Fecha,Estado,Observacion)
values (@EstudianteID,@CargaID,@fecha,@Estado,@observacion)
IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


		
IF @err = 0 
	COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar la asistencia.', 1;
		END;
END;

 

go
