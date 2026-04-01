use AgroLinkDB


--=================================
		--REUNIONES
--=================================

go

CREATE OR ALTER PROCEDURE spMAE_CrearReunion @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50)
AS
BEGIN
	begin transaction
		declare @error int = 0 ,  @err int = 0; ; 

		SELECT @error = COUNT(*)
		FROM Reunion 
		WHERE DocenteID = @docenteID 
		AND FechaHora = @fechaHora

		IF @error = 0
		BEGIN
			INSERT INTO Reunion (DocenteID, EstudianteID, FechaHora, Tema, MedioDifusion, Estado)
			VALUES (@docenteID, @estudianteID, @fechaHora, @tema, @medioDifusion, 'PROGRAMADA')
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50001, 'ERROR: Ya existe una reunión programada en la fecha y hora elegida', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;

END;

go

--PRUEBA CON Excepcion:
--EXEC spMAE_CrearReunion 6 , 3, '2026-04-01 9:00:00','Seguimiento Bajo desempeño en Clase de Grammar','VIDEOLLAMADA'

GO


CREATE OR ALTER PROCEDURE spMAE_EditarReunion @reunionID int,  @docenteID int, @estudianteID int, @fechaHora datetime,
@tema VARCHAR(255), @medioDifusion VARCHAR(50), @estado varchar(20)
AS
BEGIN
	begin transaction

		declare @currentStatus varchar(20),  @err int = 0;

		select @currentStatus = Estado  
		from Reunion where ReunionID = @reunionID

		IF ( @currentStatus = 'PROGRAMADA' )
		BEGIN
			UPDATE Reunion SET DocenteID = @docenteID , EstudianteID = @estudianteID, 
			FechaHora = @fechaHora , Tema = @tema, MedioDifusion = @medioDifusion, Estado = @estado
			Where ReunionID = @reunionID;
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50002, 'ERROR: La reunion debe estar programada para editarla', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;

END;

go
--EXEC spMAE_EditarReunion 5,7	,5, '2026-04-05 8:35:00', 'Uso adecuado de tecnología',	'VIDEOLLAMADA',	'PROGRAMADA'

-- PRUEBA CON Excepcion:   
--EXEC spMAE_EditarReunion 3,	6	,3	, '2026-05-01 9:00:00','Bajo desempeño en Clase de Grammar','VIDEOLLAMADA',	'REALIZADA'

go



--=================================
		--ACTAS
--=================================


CREATE OR ALTER PROCEDURE spMAE_CrearActa @reunionID int,  @fechaActa datetime, @acuerdos VARCHAR(MAX), 
@observaciones VARCHAR(255)
AS
BEGIN
	begin transaction
		declare @fechaReu date , @estadoReu varchar(20)	, @err int = 0; 

		SELECT @fechaReu = FechaHora, @estadoReu = Estado
		FROM Reunion 
		WHERE ReunionID = @reunionID 

		IF @estadoReu <> 'PROGRAMADA'
		BEGIN
			ROLLBACK;
			THROW 50003, 'ERROR: La reunión debe estar programada para crear acta',1;
		END;

		IF @fechaActa >= @fechaReu 
		BEGIN
			INSERT INTO Acta (ReunionID, Fecha, Acuerdos, Observaciones)
			VALUES (@reunionID, @fechaActa, @acuerdos, @observaciones)
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 

			UPDATE Reunion SET Estado = 'REALIZADA'
			WHERE ReunionID = @reunionID
			IF @@ERROR <> 0 AND @err  = 0 SELECT @err = 1 
		END
		ELSE
		BEGIN
			ROLLBACK;
			THROW 50004, 'ERROR: La fecha del acta no puede ser menor a la fecha de reunión', 1;
		END;

	IF @err = 0 
		COMMIT;
	ELSE 
		BEGIN
			ROLLBACK;
			THROW 50005, 'ERROR: Ocurrió un error al guardar los cambios.', 1;
		END;
		

END;

go
--exec spMAE_CrearActa 11, '2026-04-01', 'CLASES DE REFORZAMIENTO', 'NECESITA CONCENTRARSE'

go
SELECT * FROM Reunion
SELECT * FROM ACTA


















