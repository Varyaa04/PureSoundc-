USE [master]
GO
/****** Object:  Database [pureSound]    Script Date: 27.01.2025 23:46:06 ******/
CREATE DATABASE [pureSound]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'pureSound', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\pureSound.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'pureSound_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\pureSound_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [pureSound] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [pureSound].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [pureSound] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [pureSound] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [pureSound] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [pureSound] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [pureSound] SET ARITHABORT OFF 
GO
ALTER DATABASE [pureSound] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [pureSound] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [pureSound] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [pureSound] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [pureSound] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [pureSound] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [pureSound] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [pureSound] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [pureSound] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [pureSound] SET  DISABLE_BROKER 
GO
ALTER DATABASE [pureSound] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [pureSound] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [pureSound] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [pureSound] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [pureSound] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [pureSound] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [pureSound] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [pureSound] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [pureSound] SET  MULTI_USER 
GO
ALTER DATABASE [pureSound] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [pureSound] SET DB_CHAINING OFF 
GO
ALTER DATABASE [pureSound] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [pureSound] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [pureSound] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [pureSound] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [pureSound] SET QUERY_STORE = ON
GO
ALTER DATABASE [pureSound] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [pureSound]
GO
/****** Object:  Table [dbo].[playlistsTable]    Script Date: 27.01.2025 23:46:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[playlistsTable](
	[idPlaylist] [int] IDENTITY(1,1) NOT NULL,
	[idUser] [int] NOT NULL,
	[namePlaylist] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_playlistsTable] PRIMARY KEY CLUSTERED 
(
	[idPlaylist] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[playlistTracksTable]    Script Date: 27.01.2025 23:46:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[playlistTracksTable](
	[idPlTracks] [int] IDENTITY(1,1) NOT NULL,
	[idPlaylist] [int] NOT NULL,
	[idTracks] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_playlistTracksTable] PRIMARY KEY CLUSTERED 
(
	[idPlTracks] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tableFavourite]    Script Date: 27.01.2025 23:46:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tableFavourite](
	[favTracks] [int] IDENTITY(1,1) NOT NULL,
	[idUser] [int] NOT NULL,
	[idTrack] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_tableFavourite] PRIMARY KEY CLUSTERED 
(
	[favTracks] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usersTable]    Script Date: 27.01.2025 23:46:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usersTable](
	[idUser] [int] IDENTITY(1,1) NOT NULL,
	[userName] [nvarchar](max) NOT NULL,
	[userEmail] [nvarchar](max) NOT NULL,
	[userLogin] [nvarchar](max) NOT NULL,
	[userPassword] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_usersTable] PRIMARY KEY CLUSTERED 
(
	[idUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tableFavourite] ON 

INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (1, 1, N'2,255214E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (2, 1, N'1')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (3, 1, N'2,947516E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (4, 1, N'2,729274E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (5, 1, N'3,04756E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (6, 1, N'2,461124E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (7, 1, N'2,801558E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (8, 5, N'2,461124E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (9, 5, N'2,947516E+09')
INSERT [dbo].[tableFavourite] ([favTracks], [idUser], [idTrack]) VALUES (10, 5, N'2,729274E+09')
SET IDENTITY_INSERT [dbo].[tableFavourite] OFF
GO
SET IDENTITY_INSERT [dbo].[usersTable] ON 

INSERT [dbo].[usersTable] ([idUser], [userName], [userEmail], [userLogin], [userPassword]) VALUES (1, N'user', N'user@email.com', N'1', N'1')
INSERT [dbo].[usersTable] ([idUser], [userName], [userEmail], [userLogin], [userPassword]) VALUES (3, N'гыук2', N'йцукен"ьфшдюкг', N'2', N'2')
INSERT [dbo].[usersTable] ([idUser], [userName], [userEmail], [userLogin], [userPassword]) VALUES (5, N'test', N'ex@email.com', N'login123', N'123')
SET IDENTITY_INSERT [dbo].[usersTable] OFF
GO
ALTER TABLE [dbo].[playlistsTable]  WITH CHECK ADD  CONSTRAINT [FK_playlistsTable_usersTable] FOREIGN KEY([idUser])
REFERENCES [dbo].[usersTable] ([idUser])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[playlistsTable] CHECK CONSTRAINT [FK_playlistsTable_usersTable]
GO
ALTER TABLE [dbo].[playlistTracksTable]  WITH CHECK ADD  CONSTRAINT [FK_playlistTracksTable_playlistsTable] FOREIGN KEY([idPlaylist])
REFERENCES [dbo].[playlistsTable] ([idPlaylist])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[playlistTracksTable] CHECK CONSTRAINT [FK_playlistTracksTable_playlistsTable]
GO
ALTER TABLE [dbo].[tableFavourite]  WITH CHECK ADD  CONSTRAINT [FK_tableFavourite_usersTable] FOREIGN KEY([idUser])
REFERENCES [dbo].[usersTable] ([idUser])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tableFavourite] CHECK CONSTRAINT [FK_tableFavourite_usersTable]
GO
USE [master]
GO
ALTER DATABASE [pureSound] SET  READ_WRITE 
GO
