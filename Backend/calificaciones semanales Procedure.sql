use AgroLinkDB


select E.EstudianteID,E.Nombre as Estudiante,G.NombreGrado as Grado,s.Letra as Seccion,A.Descripcion,
ASI.Nombre as Asignatura,A.Parcial,A.Valor as Valor,C.Nota as calificacion from 
Calificacion C inner join Actividad A on C.ActividadID=A.ActividadID 
inner join CargaAcademica CA on A.CargaID=CA.CargaID
inner join Seccion S on CA.SeccionID=S.SeccionID
inner join Grado G on S.GradoID=G.GradoID
inner join Estudiante E on C.EstudianteID=E.EstudianteID
inner join Asignatura Asi on CA.AsignaturaID=Asi.AsignaturaID
where g.NombreGrado='PRIMERO' and a.Parcial= 1 and s.letra='A' and asi.Nombre like '%Math%' AND c.Fecha BETWEEN '2026-03-20' AND '2026-03-22'


select * from Calificacion


-- rpocedimiento nuevo


go
create or alter procedure spMAE_Calificaciones_semanlaes @Grado Varchar(50),@parcial int,@Seccion varchar(10),@Asignatura varchar(70),@fecha_inicial DATE,@fecha_final DATE
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
and c.Fecha BETWEEN @fecha_inicial AND @fecha_final
and (@Estudiante IS NULL OR E.Nombre LIKE '%' + @Estudiante + '%')

end