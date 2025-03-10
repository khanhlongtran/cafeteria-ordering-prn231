-- CREATE DATABASE [CafeteriaOrderingDB]
-- GO

USE [CafeteriaOrderingDB]
GO

/****** Object:  Table [account_activity]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [account_activity](
	[activity_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[activity_type] [varchar](100) NULL,
	[activity_time] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[activity_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [addresses]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [addresses](
	[address_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[address_line] [varchar](255) NOT NULL,
	[city] [varchar](100) NULL,
	[state] [varchar](100) NULL,
	[zip_code] [varchar](20) NULL,
	[is_default] [bit] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[address_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [delivery]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [delivery](
	[delivery_id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NOT NULL,
	[deliver_user_id] [int] NOT NULL,
	[pickup_time] [datetime] NULL,
	[delivery_time] [datetime] NULL,
	[delivery_status] [varchar](50) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[delivery_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [feedback]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [feedback](
	[feedback_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[order_id] [int] NOT NULL,
	[rating] [int] NULL,
	[comment] [varchar](max) NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [menu_items]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [menu_items](
	[item_id] [int] IDENTITY(1,1) NOT NULL,
	[menu_id] [int] NOT NULL,
	[item_name] [varchar](100) NOT NULL,
	[description] [varchar](255) NULL,
	[price] [decimal](10, 2) NOT NULL,
	[item_type] [varchar](50) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [menus]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [menus](
	[menu_id] [int] IDENTITY(1,1) NOT NULL,
	[manager_id] [int] NOT NULL,
	[menu_name] [varchar](100) NOT NULL,
	[description] [varchar](255) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[menu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [order_items]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [order_items](
	[order_item_id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NOT NULL,
	[item_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[order_item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [orders]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [orders](
	[order_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[order_date] [datetime] NOT NULL,
	[status] [varchar](50) NOT NULL,
	[payment_method] [varchar](50) NULL,
	[total_amount] [decimal](10, 2) NOT NULL,
	[address_id] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [recommended_meals]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [recommended_meals](
	[recommend_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[item_id] [int] NOT NULL,
	[score] [decimal](5, 2) NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[recommend_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [revenue_reports]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [revenue_reports](
	[report_id] [int] IDENTITY(1,1) NOT NULL,
	[manager_id] [int] NOT NULL,
	[report_date] [date] NOT NULL,
	[total_orders] [int] NOT NULL,
	[total_revenue] [decimal](10, 2) NOT NULL,
	[generated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[report_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [users]    Script Date: 2/28/2025 9:22:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[full_name] [varchar](100) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[password] [varchar](255) NOT NULL,
	[phone] [varchar](20) NULL,
	[default_cuisine] [varchar](50) NULL,
	[role] [varchar](20) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__users__AB6E6164380A30B7]    Script Date: 2/28/2025 9:22:46 AM ******/
ALTER TABLE [users] ADD UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [account_activity] ADD  DEFAULT (getdate()) FOR [activity_time]
GO
ALTER TABLE [addresses] ADD  DEFAULT ((0)) FOR [is_default]
GO
ALTER TABLE [addresses] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [addresses] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [delivery] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [delivery] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [feedback] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [menu_items] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [menu_items] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [menus] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [menus] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [orders] ADD  DEFAULT (getdate()) FOR [order_date]
GO
ALTER TABLE [orders] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [orders] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [recommended_meals] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [revenue_reports] ADD  DEFAULT (getdate()) FOR [generated_at]
GO
ALTER TABLE [users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [users] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [account_activity]  WITH CHECK ADD  CONSTRAINT [FK_account_activity_users] FOREIGN KEY([user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [account_activity] CHECK CONSTRAINT [FK_account_activity_users]
GO
ALTER TABLE [addresses]  WITH CHECK ADD  CONSTRAINT [FK_addresses_users] FOREIGN KEY([user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [addresses] CHECK CONSTRAINT [FK_addresses_users]
GO
ALTER TABLE [delivery]  WITH CHECK ADD  CONSTRAINT [FK_delivery_orders] FOREIGN KEY([order_id])
REFERENCES [orders] ([order_id])
GO
ALTER TABLE [delivery] CHECK CONSTRAINT [FK_delivery_orders]
GO
ALTER TABLE [delivery]  WITH CHECK ADD  CONSTRAINT [FK_delivery_users] FOREIGN KEY([deliver_user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [delivery] CHECK CONSTRAINT [FK_delivery_users]
GO
ALTER TABLE [feedback]  WITH CHECK ADD  CONSTRAINT [FK_feedback_orders] FOREIGN KEY([order_id])
REFERENCES [orders] ([order_id])
GO
ALTER TABLE [feedback] CHECK CONSTRAINT [FK_feedback_orders]
GO
ALTER TABLE [feedback]  WITH CHECK ADD  CONSTRAINT [FK_feedback_users] FOREIGN KEY([user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [feedback] CHECK CONSTRAINT [FK_feedback_users]
GO
ALTER TABLE [menu_items]  WITH CHECK ADD  CONSTRAINT [FK_menu_items_menus] FOREIGN KEY([menu_id])
REFERENCES [menus] ([menu_id])
GO
ALTER TABLE [menu_items] CHECK CONSTRAINT [FK_menu_items_menus]
GO
ALTER TABLE [menus]  WITH CHECK ADD  CONSTRAINT [FK_menus_users] FOREIGN KEY([manager_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [menus] CHECK CONSTRAINT [FK_menus_users]
GO
ALTER TABLE [order_items]  WITH CHECK ADD  CONSTRAINT [FK_order_items_menu_items] FOREIGN KEY([item_id])
REFERENCES [menu_items] ([item_id])
GO
ALTER TABLE [order_items] CHECK CONSTRAINT [FK_order_items_menu_items]
GO
ALTER TABLE [order_items]  WITH CHECK ADD  CONSTRAINT [FK_order_items_orders] FOREIGN KEY([order_id])
REFERENCES [orders] ([order_id])
GO
ALTER TABLE [order_items] CHECK CONSTRAINT [FK_order_items_orders]
GO
ALTER TABLE [orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_addresses] FOREIGN KEY([address_id])
REFERENCES [addresses] ([address_id])
GO
ALTER TABLE [orders] CHECK CONSTRAINT [FK_orders_addresses]
GO
ALTER TABLE [orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_users] FOREIGN KEY([user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [orders] CHECK CONSTRAINT [FK_orders_users]
GO
ALTER TABLE [recommended_meals]  WITH CHECK ADD  CONSTRAINT [FK_recommended_meals_items] FOREIGN KEY([item_id])
REFERENCES [menu_items] ([item_id])
GO
ALTER TABLE [recommended_meals] CHECK CONSTRAINT [FK_recommended_meals_items]
GO
ALTER TABLE [recommended_meals]  WITH CHECK ADD  CONSTRAINT [FK_recommended_meals_users] FOREIGN KEY([user_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [recommended_meals] CHECK CONSTRAINT [FK_recommended_meals_users]
GO
ALTER TABLE [revenue_reports]  WITH CHECK ADD  CONSTRAINT [FK_revenue_reports_users] FOREIGN KEY([manager_id])
REFERENCES [users] ([user_id])
GO
ALTER TABLE [revenue_reports] CHECK CONSTRAINT [FK_revenue_reports_users]
GO
ALTER TABLE [users]  WITH CHECK ADD CHECK  (([role]='MANAGER' OR [role]='DELIVER' OR [role]='PATRON'))
GO
