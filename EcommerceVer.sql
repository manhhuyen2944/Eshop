USE [EcommerceVer]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[FullName] [nvarchar](max) NULL,
	[IsAdmin] [bit] NOT NULL,
	[Avatar] [nvarchar](max) NULL,
	[Status] [bit] NOT NULL,
	[RandomKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Carts]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Carts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Carts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvoiceDetails]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [int] NOT NULL,
 CONSTRAINT [PK_InvoiceDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](max) NULL,
	[AccountId] [int] NOT NULL,
	[IssuedDate] [datetime2](7) NOT NULL,
	[ShippingAddress] [nvarchar](max) NULL,
	[ShippingPhone] [nvarchar](max) NULL,
	[Total] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SKU] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [int] NOT NULL,
	[Stock] [int] NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductTypes]    Script Date: 12/21/2022 11:02:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_ProductTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20221126163921_init', N'6.0.10')
SET IDENTITY_INSERT [dbo].[Accounts] ON 

INSERT [dbo].[Accounts] ([Id], [Username], [Password], [Email], [Phone], [Address], [FullName], [IsAdmin], [Avatar], [Status], [RandomKey]) VALUES (1, N'nobita', N'cf745a39a43617e62378aada34c06ebc', N'nobita@gmail.com', N'0345888999', N'TP HCM', N'NOBITA', 1, N'nobita.jpg', 1, N'z{tp*')
INSERT [dbo].[Accounts] ([Id], [Username], [Password], [Email], [Phone], [Address], [FullName], [IsAdmin], [Avatar], [Status], [RandomKey]) VALUES (2, N'doremon', N'98a4bc401a280f859917e6afe15ae00a', N'doremon@gmail.com', N'0345777999', N'MY THO', N'DOREMON', 0, N'doremon.jpg', 1, N'[au87')
INSERT [dbo].[Accounts] ([Id], [Username], [Password], [Email], [Phone], [Address], [FullName], [IsAdmin], [Avatar], [Status], [RandomKey]) VALUES (3, N'chaien', N'af630e0ae9452c497e1b3f893c2ca829', N'chaien@gmail.com', N'0345666999', N'HA NOI', N'CHAIEN', 0, N'chaien.jpg', 1, N'![b1a')
INSERT [dbo].[Accounts] ([Id], [Username], [Password], [Email], [Phone], [Address], [FullName], [IsAdmin], [Avatar], [Status], [RandomKey]) VALUES (4, N'user123', N'114d883cdfd93395c5454223e69851b3', N'user123@gmail.com', N'0345111222', N'ABC', N'User123', 0, N'user1.jpg', 1, N'%^f]p')
SET IDENTITY_INSERT [dbo].[Accounts] OFF
SET IDENTITY_INSERT [dbo].[InvoiceDetails] ON 

INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (1, 1, 3, 1, 1000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (2, 1, 2, 1, 30000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (3, 1, 1, 1, 2000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (4, 2, 3, 1, 1000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (5, 2, 2, 1, 30000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (6, 2, 1, 1, 2000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (7, 3, 3, 1, 1000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (8, 3, 2, 1, 30000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (9, 3, 1, 1, 2000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (10, 4, 3, 1, 1000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (11, 4, 2, 1, 30000000)
INSERT [dbo].[InvoiceDetails] ([Id], [InvoiceId], [ProductId], [Quantity], [UnitPrice]) VALUES (12, 4, 1, 1, 2000000)
SET IDENTITY_INSERT [dbo].[InvoiceDetails] OFF
SET IDENTITY_INSERT [dbo].[Invoices] ON 

INSERT [dbo].[Invoices] ([Id], [Code], [AccountId], [IssuedDate], [ShippingAddress], [ShippingPhone], [Total], [Status]) VALUES (1, N'20221128030433', 3, CAST(N'2022-11-28 15:04:33.0000000' AS DateTime2), N'America', N'0345666111', 33000000, 1)
INSERT [dbo].[Invoices] ([Id], [Code], [AccountId], [IssuedDate], [ShippingAddress], [ShippingPhone], [Total], [Status]) VALUES (2, N'20221128031907', 3, CAST(N'2022-11-28 15:19:07.2636131' AS DateTime2), N'HA NOI', N'0345666999', 33000000, 0)
INSERT [dbo].[Invoices] ([Id], [Code], [AccountId], [IssuedDate], [ShippingAddress], [ShippingPhone], [Total], [Status]) VALUES (3, N'20221128031953', 2, CAST(N'2022-11-28 15:19:53.0000000' AS DateTime2), N'MY THO', N'0345777999', 33000000, 1)
INSERT [dbo].[Invoices] ([Id], [Code], [AccountId], [IssuedDate], [ShippingAddress], [ShippingPhone], [Total], [Status]) VALUES (4, N'20221130044729', 3, CAST(N'2022-11-30 16:47:29.9255827' AS DateTime2), N'HA NOI 123', N'0345666123', 33000000, 0)
SET IDENTITY_INSERT [dbo].[Invoices] OFF
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [SKU], [Name], [Description], [Price], [Stock], [ProductTypeId], [Image], [Status]) VALUES (1, N'KWRIJ1DNJM', N'Iphone 14 Promax', N'ABC', 2000000, 99, 2, N'iphone14promax.png', 1)
INSERT [dbo].[Products] ([Id], [SKU], [Name], [Description], [Price], [Stock], [ProductTypeId], [Image], [Status]) VALUES (2, N'0RDYXMBEIG', N'Laptop Acer Nitro 6', N'XYZ', 30000000, 80, 1, N'laptopacernitro6.jpg', 1)
INSERT [dbo].[Products] ([Id], [SKU], [Name], [Description], [Price], [Stock], [ProductTypeId], [Image], [Status]) VALUES (3, N'B2GBBH2T4U', N'Airpod', N'HEHE', 1000000, 69, 3, N'airpod.jpg', 1)
SET IDENTITY_INSERT [dbo].[Products] OFF
SET IDENTITY_INSERT [dbo].[ProductTypes] ON 

INSERT [dbo].[ProductTypes] ([Id], [Name], [Status]) VALUES (1, N'Laptop', 1)
INSERT [dbo].[ProductTypes] ([Id], [Name], [Status]) VALUES (2, N'Phone', 1)
INSERT [dbo].[ProductTypes] ([Id], [Name], [Status]) VALUES (3, N'Phụ Kiện', 1)
SET IDENTITY_INSERT [dbo].[ProductTypes] OFF
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_Carts_Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_Carts_Accounts_AccountId]
GO
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_Carts_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_Carts_Products_ProductId]
GO
ALTER TABLE [dbo].[InvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetails_Invoices_InvoiceId] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InvoiceDetails] CHECK CONSTRAINT [FK_InvoiceDetails_Invoices_InvoiceId]
GO
ALTER TABLE [dbo].[InvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetails_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InvoiceDetails] CHECK CONSTRAINT [FK_InvoiceDetails_Products_ProductId]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Accounts_AccountId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ProductTypes_ProductTypeId] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[ProductTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ProductTypes_ProductTypeId]
GO
