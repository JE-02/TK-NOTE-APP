CREATE TABLE notes(
	noteId INT IDENTITY NOT NULL PRIMARY KEY,
	noteTitle varchar(255) NOT NULL,
	userId int NOT NULL FOREIGN KEY REFERENCES usersInfo(userId)
);

ALTER TABLE notes
ADD noteTitle varchar(255) NOT NULL, noteContent varchar(500) NOT NULL;



SELECT * FROM notes;
