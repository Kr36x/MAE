use AgroLinkDB

select * from CargaAcademica
select * from Grado

--1.- Asistencias diariasas
select D.Nombre as Docente,G.Nivel as Grado,S.Letra as Seccion,A.Fecha as Fecha,E.Nombre as Estudiante, A.Estado as Estado,
isnull( A.Observacion,'Sin observaciones') as Observaciones
from Asistencia A inner join CargaAcademica CA on A.CargaID=CA.CargaID 
inner join Estudiante E on A.EstudianteID=E.EstudianteID
inner join Seccion S on CA.SeccionID=S.SeccionID
inner join Grado G on G.GradoID=S.GradoID
inner join Docente D on CA.DocenteID=d.DocenteID
where a.Fecha between '2026-03-02' and '2026-03-03' and d.Nombre='JUAN PEREZ'
order by a.Fecha ASC,E.Nombre ASC

go
create or alter procedure spMAE_Asistencias_por_Grado @fecha_inicial DATE,@fecha_final DATE = NULL,@Docente varchar(100),@Estudiante varchar(100)=Null 
as
begin

select D.Nombre as Docente,G.Nivel as Grado,S.Letra as Seccion,A.Fecha as Fecha,E.Nombre as Estudiante, A.Estado as Estado,
isnull( A.Observacion,'Sin observaciones') as Observaciones
from Asistencia A inner join CargaAcademica CA on A.CargaID=CA.CargaID 
inner join Estudiante E on A.EstudianteID=E.EstudianteID
inner join Seccion S on CA.SeccionID=S.SeccionID
inner join Grado G on G.GradoID=S.GradoID
inner join Docente D on CA.DocenteID=d.DocenteID
where (
          -- sin fecha final
          (@fecha_final IS NULL AND A.Fecha = @fecha_inicial)
          OR 
          -- ranfo de fechas
          (@fecha_final IS NOT NULL AND A.Fecha BETWEEN @fecha_inicial AND @fecha_final)
      ) and d.Nombre=@Docente 
      and (@Estudiante IS NULL OR E.Nombre LIKE '%' + @Estudiante + '%')
order by a.Fecha ASC,E.Nombre ASC
end

go

--probando
EXEC spMAE_Asistencias_por_Grado @fecha_inicial='2026-03-02',@fecha_final='2026-03-04',@Docente='JUAN PEREZ', @Estudiante='Alexis'



