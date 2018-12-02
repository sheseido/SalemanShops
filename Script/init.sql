CREATE DATABASE IF NOT EXISTS `mo.filter.com` DEFAULT CHARSET utf8;

DROP TABLE IF EXISTS `salemaninfo`;
CREATE TABLE `salemaninfo`
(
	`Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT COMMENT '主键',
    `Name` VARCHAR(50) NOT NULL COMMENT '业务员姓名',
    `Mobile` VARCHAR(20)  COMMENT '手机号',
    `SettlementPrice` DECIMAL(11,2) NOT NULL COMMENT '结算价',
	`CreatedAt` DATETIME NOT NULL,
    `CreatedBy` INT NOT NULL,
    `UpdatedAt` DATETIME,
    `UpdatedBy` INT,
	`IsDelete` BIT NOT NULL DEFAULT 0,
    `DeletedAt` DATETIME,
    `DeletedBy` INT,
    UNIQUE KEY uk_salemaninfo_Name(`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8 COLLATE=UTF8_UNICODE_CI COMMENT '业务员信息表';

DROP TABLE IF EXISTS `shopinfo`;
CREATE TABLE `shopinfo`
(
	`Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT COMMENT '主键',
    `Name` VARCHAR(50) NOT NULL COMMENT '店铺名称',
    `Contact` VARCHAR(50) COMMENT '联系人',
    `Mobile` VARCHAR(20)  COMMENT '手机号',
    `SalemanId` INT NOT NULL COMMENT '所属业务员Id',
	`CreatedAt` DATETIME NOT NULL,
    `CreatedBy` INT NOT NULL,
    `UpdatedAt` DATETIME,
    `UpdatedBy` INT,
	`IsDelete` BIT NOT NULL DEFAULT 0,
    `DeletedAt` DATETIME,
    `DeletedBy` INT,
    UNIQUE KEY uk_shopinfo_Name(`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8 COLLATE=UTF8_UNICODE_CI COMMENT '店铺信息表';

DROP TABLE IF EXISTS `waybillinfo`;
CREATE TABLE `waybillinfo`
(
	`Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT COMMENT '主键',
    `Code` VARCHAR(50) NOT NULL COMMENT '运单号',
    `Time` VARCHAR(20) COMMENT '日期',
    `ShopId` INT NOT NULL COMMENT '店铺Id',
    `SalemanId` INT NOT NULL COMMENT '业务员',
    `Province` VARCHAR(20) COMMENT '省',
    `City` VARCHAR(50) COMMENT '城市',
    `Weight` DOUBLE COMMENT '重量',
	`CreatedAt` DATETIME NOT NULL,
    `CreatedBy` INT NOT NULL,
    `UpdatedAt` DATETIME,
    `UpdatedBy` INT,
	`IsDelete` BIT NOT NULL DEFAULT 0,
    `DeletedAt` DATETIME,
    `DeletedBy` INT,
    UNIQUE KEY uk_waybillinfo_Code(`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8 COLLATE=UTF8_UNICODE_CI COMMENT '运单信息表';