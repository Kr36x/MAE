use AgroLinkDB

select * from Grado
select * from Seccion

--==========================
--		 Crear Grados
--==========================
go
create or alter procedure  spMAE_crearGrados
--valores necesarios
@Nombre varchar(100), @Nivel varchar(50), @Estado int 
as

begin 

begin transaction
declare  @err int = 0,  @duplicado int = 0;

--validar si ya existe un grado 
select @duplicado=Count(*) from Grado where NombreGrado=@Nombre

if @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 50006, 'ERROR: Ya existe Este Grado', 1;
		END;

insert into Grado (NombreGrado,Nivel,Estado)
values (@Nombre,@Nivel,@Estado)
--error al ingresar los datos
IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


IF @err = 0 
	COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;


end;




--==========================
--		 Crear Seccion
--==========================

select * from Seccion

go
create or alter procedure spMAE_CrearSecciones
@gradoNombre int, @letra varchar(10),@Turno varchar(60),@Aula int
as
begin

begin transaction
declare @err int = 0,@duplicado int = 0,@gradoID int;

select @duplicado=count(*) from seccion s inner join Grado g on s.GradoID=g.GradoID where g.NombreGrado=@gradoNombre and s.Letra=@letra

--obtener el id del grado
select @gradoID=g.GradoID from seccion s inner join Grado g on s.GradoID=g.GradoID where g.NombreGrado=@gradoNombre

if @duplicado > 0
		BEGIN
			ROLLBACK;
			THROW 50006, 'ERROR: Ya existe Este Grado', 1;
		END;
--si no hay duplicados
insert into Seccion(GradoID,Letra,Turno,Aula)
values(@gradoID,@letra,@Turno,@Aula)
IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 ;


IF @err = 0 
	COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;



end;


