USE notes;

CREATE TABLE usersInfo(
	userId INT IDENTITY NOT NULL PRIMARY KEY,
	userName varchar(100) NOT NULL UNIQUE,
	userPassword char(60) NOT NULL UNIQUE
);

SELECT * FROM usersInfo;