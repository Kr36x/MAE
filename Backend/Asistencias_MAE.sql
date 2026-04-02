use AgroLinkDB

select * from Asistencia
select * from Estudiante
select * from CargaAcademica
select * from Asignatura

go

create or alter procedure sp_Ingresar_Asistencias 
-- Datos a Ingresar
@grado varchar(100),@seccion varchar(50),@fecha date,@Asignatura varchar(20),@Estado varchar(50),@observacion varchar(50)=NULL
as
BEGIN
 begin transaction
 declare @err int = 0,  @duplicado int = 0; 
		


 end

go
