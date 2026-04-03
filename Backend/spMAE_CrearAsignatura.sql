use AgroLinkDB


select * from Asignatura

go

create or alter procedure spMAE_CrearAsignatura 
@Nombre varchar(60),@Area varchar(100),@descripcion varchar(150)
as
begin
begin transaction

declare @err int = 0,  @duplicado int = 0; 

--verificar si viene un duplicado
select @duplicado=count(*) from Asignatura where Nombre=@Nombre -- no seria mejor un like ?

IF @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 50006, 'ERROR: Ya existe Esta Asignatura 1.', 1;
		END;
--ingresar los datos

insert into Asignatura(Nombre,Area,Descripcion)
values(@Nombre,@Area,@descripcion)

IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


IF @err = 0 
	COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al querer agrear los datos.', 1;
		END;


end;