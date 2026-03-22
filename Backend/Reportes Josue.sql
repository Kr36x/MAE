use AgroLinkDB

select * from CargaAcademica
select * from Docente

--2.- Asistencias diariasas

go
create or alter procedure spMAE_Asistencias_por_Grado @fecha_inicial DATE,@fecha_final DATE = NULL,@Docente int,@Estudiante varchar(100)=Null ,@grado varchar(100),
@Seccion varchar(10)
as
begin

select D.Nombre as Docente,G.NombreGrado as Grado,S.Letra as Seccion,A.Fecha as Fecha,E.Nombre as Estudiante, A.Estado as Estado,
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
      ) and d.UsuarioID=@Docente 
      and (@Estudiante IS NULL OR E.Nombre LIKE '%' + @Estudiante + '%')
      and  g.NombreGrado=@Grado and 
        s.Letra=@Seccion 
order by a.Fecha ASC,E.Nombre ASC
end

go

--probando
EXEC spMAE_Asistencias_por_Grado @fecha_inicial='2026-03-02',@fecha_final='2026-03-04',@Docente=4,@Grado='Segundo',@Seccion='A',@Estudiante='ANGEL GABRIEL RIVERA'


-- 3.- calificacioens semanales

select E.EstudianteID,E.Nombre as Estudiante,G.NombreGrado as Grado,s.Letra as Seccion,A.Descripcion,
ASI.Nombre as Asignatura,A.Parcial,A.Valor as Valor,C.Nota as calificacion,Year(C.Fecha) as Academic_Year from 
Calificacion C inner join Actividad A on C.ActividadID=A.ActividadID 
inner join CargaAcademica CA on A.CargaID=CA.CargaID
inner join Seccion S on CA.SeccionID=S.SeccionID
inner join Grado G on S.GradoID=G.GradoID
inner join Estudiante E on C.EstudianteID=E.EstudianteID
inner join Asignatura Asi on CA.AsignaturaID=Asi.AsignaturaID
where g.NombreGrado='PRIMERO' and a.Parcial= 1 and s.letra='A' and asi.Nombre like '%Math%' and year(c.Fecha) = 2026



go
create or alter procedure spMAE_Calificaciones_semanlaes @Grado Varchar(50),@parcial int,@Seccion varchar(10),@Asignatura varchar(70),@anio int
,@Estudiante varchar(100)=Null
as
begin

select E.EstudianteID,E.Nombre as Estudiante,G.NombreGrado as Grado,s.Letra as Seccion,A.Descripcion,
ASI.Nombre as Asignatura,A.Parcial,A.Valor as Valor,C.Nota as calificacion from 
Calificacion C inner join Actividad A on C.ActividadID=A.ActividadID 
inner join CargaAcademica CA on A.CargaID=CA.CargaID
inner join Seccion S on CA.SeccionID=S.SeccionID
inner join Grado G on S.GradoID=G.GradoID
inner join Estudiante E on C.EstudianteID=E.EstudianteID
inner join Asignatura Asi on CA.AsignaturaID=Asi.AsignaturaID
where g.NombreGrado=@Grado and 
a.Parcial= @parcial 
and s.letra=@Seccion 
and asi.Nombre like '%' + @Asignatura + '%' 
and year(c.Fecha) = @anio
and (@Estudiante IS NULL OR E.Nombre LIKE '%' + @Estudiante + '%')

end

go
EXEC spMAE_Calificaciones_semanlaes @Grado='Primero',@parcial= 1,@Seccion='A',@Asignatura = 'Math',@anio =2026,@Estudiante='BRAYAN JOSUE ESPINAL'
