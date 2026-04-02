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


end;

go


