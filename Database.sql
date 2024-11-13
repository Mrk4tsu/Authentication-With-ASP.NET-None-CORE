﻿CREATE TABLE Users(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Username VARCHAR(50) UNIQUE NOT NULL,
	Password VARCHAR(50) NOT NULL,
	FullName NVARCHAR(150) NOT NULL,
	Email VARCHAR(50) NOT NULL,
	Phone VARCHAR(15) NOT NULL,
	CreatedDate DATETIME NOT NULL,
	ModifiedDate DATETIME NOT NULL,
	UserCODE VARCHAR(20) NOT NULL,
	Status BIT NOT NULL,
	IsVerifedEmail BIT NOT NULL
);
CREATE TABLE UserDevice(
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
	DeviceName NVARCHAR(50) NOT NULL,
	IpAddress VARCHAR(50) NOT NULL,
	FistLogin DATETIME NOT NULL,
	LastLogin DATETIME NOT NULL,
	IsTrusted BIT NOT NULL
);
CREATE TABLE DeviceVerificationToken(
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
	Token VARCHAR(10) NOT NULL,
	CreateTime DATETIME NOT NULL,
	ExpiredTime DATETIME NOT NULL,
	Status BIT NOT NULL
);
CREATE TABLE LoginLog(
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
	DeviceId INT NOT NULL FOREIGN KEY REFERENCES UserDevice(Id),
	LoginTime DATETIME NOT NULL,
	IpAddress VARCHAR(50) NOT NULL,
	IsNewDevice BIT NOT NULL,
	Status BIT NOT NULL,
);