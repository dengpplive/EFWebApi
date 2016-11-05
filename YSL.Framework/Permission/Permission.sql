CREATE TABLE [dbo].[SysModule](
    [Id] [varchar](50) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [EnglishName] [varchar](200) NULL,
    [ParentId] [varchar](50) NULL,
    [Url] [varchar](200) NULL,
    [Iconic] [varchar](200) NULL,
    [Sort] [int] NULL,
    [Remark] [varchar](4000) NULL,
    [State] [bit] NULL,
    [CreatePerson] [varchar](200) NULL,
    [CreateTime] [datetime] NULL,
    [IsLast] [bit] NOT NULL,
    [Version] [timestamp] NULL,
 CONSTRAINT [PK_SysModule] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[SysModuleOperate](
    [Id] [varchar](200) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [KeyCode] [varchar](200) NOT NULL,
    [ModuleId] [varchar](50) NOT NULL,
    [IsValid] [bit] NOT NULL,
    [Sort] [int] NOT NULL,
 CONSTRAINT [PK_SysModuleOperate] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[SysRole](
    [Id] [varchar](50) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](4000) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [CreatePerson] [varchar](200) NOT NULL,
 CONSTRAINT [PK_SysRole] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[SysUser](
    [Id] [varchar](50) NOT NULL,
    [UserName] [varchar](200) NOT NULL,
    [Password] [varchar](200) NOT NULL,
    [TrueName] [varchar](200) NULL,
    [Card] [varchar](50) NULL,
    [MobileNumber] [varchar](200) NULL,
    [PhoneNumber] [varchar](200) NULL,
    [QQ] [varchar](50) NULL,
    [EmailAddress] [varchar](200) NULL,
    [OtherContact] [varchar](200) NULL,
    [Province] [varchar](200) NULL,
    [City] [varchar](200) NULL,
    [Village] [varchar](200) NULL,
    [Address] [varchar](200) NULL,
    [State] [bit] NULL,
    [CreateTime] [datetime] NULL,
    [CreatePerson] [varchar](200) NULL,
    [Sex] [varchar](10) NULL,
    [Birthday] [datetime] NULL,
    [JoinDate] [datetime] NULL,
    [Marital] [varchar](10) NULL,
    [Political] [varchar](50) NULL,
    [Nationality] [varchar](20) NULL,
    [Native] [varchar](20) NULL,
    [School] [varchar](50) NULL,
    [Professional] [varchar](100) NULL,
    [Degree] [varchar](20) NULL,
    [DepId] [varchar](50) NOT NULL,
    [PosId] [varchar](50) NOT NULL,
    [Expertise] [varchar](3000) NULL,
    [JobState] [varchar](20) NULL,
    [Photo] [varchar](200) NULL,
    [Attach] [varchar](200) NULL,
 CONSTRAINT [PK_SysUser] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份证' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'MobileNumber'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'婚姻' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Marital'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'党派' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Political'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'民族' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Nationality'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'籍贯' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Native'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'毕业学校' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'School'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'就读专业' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Professional'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'学历' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Degree'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'DepId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'职位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'PosId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'个人简介' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Expertise'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'在职状况' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'JobState'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'照片' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Photo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Attach'
GO


CREATE TABLE [dbo].[SysRoleSysUser](
    [SysUserId] [varchar](50) NOT NULL,
    [SysRoleId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SysRoleSysUser] PRIMARY KEY CLUSTERED 
(
    [SysUserId] ASC,
    [SysRoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[SysRight](
    [Id] [varchar](200) NOT NULL,
    [ModuleId] [varchar](50) NOT NULL,
    [RoleId] [varchar](50) NOT NULL,
    [Rightflag] [bit] NOT NULL,
 CONSTRAINT [PK_SysRight] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[SysRightOperate](
    [Id] [varchar](200) NOT NULL,
    [RightId] [varchar](200) NOT NULL,
    [KeyCode] [varchar](200) NOT NULL,
    [IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_SysRightOperate] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[SysModule]  WITH NOCHECK ADD  CONSTRAINT [FK_SysModule_SysModule] FOREIGN KEY([ParentId])
REFERENCES [dbo].[SysModule] ([Id])
GO

ALTER TABLE [dbo].[SysModule] NOCHECK CONSTRAINT [FK_SysModule_SysModule]
GO

ALTER TABLE [dbo].[SysModuleOperate]  WITH CHECK ADD  CONSTRAINT [FK_SysModuleOperate_SysModule] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[SysModule] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysModuleOperate] CHECK CONSTRAINT [FK_SysModuleOperate_SysModule]
GO


ALTER TABLE [dbo].[SysRoleSysUser]  WITH CHECK ADD  CONSTRAINT [FK_SysRoleSysUser_SysRole] FOREIGN KEY([SysRoleId])
REFERENCES [dbo].[SysRole] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysRoleSysUser] CHECK CONSTRAINT [FK_SysRoleSysUser_SysRole]
GO

ALTER TABLE [dbo].[SysRoleSysUser]  WITH CHECK ADD  CONSTRAINT [FK_SysRoleSysUser_SysUser] FOREIGN KEY([SysUserId])
REFERENCES [dbo].[SysUser] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysRoleSysUser] CHECK CONSTRAINT [FK_SysRoleSysUser_SysUser]
GO

ALTER TABLE [dbo].[SysRight]  WITH CHECK ADD  CONSTRAINT [FK_SysRight_SysModule] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[SysModule] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysRight] CHECK CONSTRAINT [FK_SysRight_SysModule]
GO

ALTER TABLE [dbo].[SysRight]  WITH CHECK ADD  CONSTRAINT [FK_SysRight_SysRole] FOREIGN KEY([RoleId])
REFERENCES [dbo].[SysRole] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysRight] CHECK CONSTRAINT [FK_SysRight_SysRole]
GO
ALTER TABLE [dbo].[SysRightOperate]  WITH CHECK ADD  CONSTRAINT [FK_SysRightOperate_SysRight] FOREIGN KEY([RightId])
REFERENCES [dbo].[SysRight] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SysRightOperate] CHECK CONSTRAINT [FK_SysRightOperate_SysRight]
GO