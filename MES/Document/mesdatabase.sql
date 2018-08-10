-- phpMyAdmin SQL Dump
-- version phpStudy 2014
-- http://www.phpmyadmin.net
--
-- 主机: localhost
-- 生成日期: 2016 年 08 月 02 日 11:03
-- 服务器版本: 5.5.40
-- PHP 版本: 5.3.29

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- 数据库: `mesdatabase`
--

-- --------------------------------------------------------

--
-- 表的结构 `mes_admin`
--

CREATE TABLE IF NOT EXISTS `mes_admin` (
  `user_no` int(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(16) NOT NULL,
  `password` varchar(16) NOT NULL,
  `level` int(10) NOT NULL DEFAULT '1',
  `other` text,
  PRIMARY KEY (`user_no`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='用户表' AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `mes_agency`
--

CREATE TABLE IF NOT EXISTS `mes_agency` (
  `agency_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '中介编号',
  `correlation_no` int(10) NOT NULL COMMENT '关联编号',
  `situation_no` int(10) NOT NULL COMMENT '状态编号',
  `counts` int(10) NOT NULL COMMENT '当前生产数',
  `createdate` datetime NOT NULL COMMENT '创建时间',
  `islast` int(2) NOT NULL DEFAULT '0' COMMENT '是否为最终报文',
  `others` text COMMENT '备注',
  PRIMARY KEY (`agency_no`),
  KEY `key1` (`correlation_no`),
  KEY `key2` (`situation_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='报文中介' AUTO_INCREMENT=59 ;

--
-- 转存表中的数据 `mes_agency`
--

INSERT INTO `mes_agency` (`agency_no`, `correlation_no`, `situation_no`, `counts`, `createdate`, `islast`, `others`) VALUES
(58, 68, 1, 3, '2016-08-02 11:00:16', 0, NULL),
(13, 23, 1, 55, '2016-07-25 17:42:34', 0, NULL),
(15, 22, 1, 57, '2016-07-26 15:06:40', 0, NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_collect`
--

CREATE TABLE IF NOT EXISTS `mes_collect` (
  `collect_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '批次编号',
  `correlation_no` int(10) NOT NULL COMMENT '关联号',
  `machine_no` int(10) NOT NULL COMMENT '机器编号',
  `products_no` int(10) NOT NULL COMMENT '产品编号',
  `staff_no` int(10) DEFAULT '0' COMMENT '员工编号',
  `plans_no` int(10) NOT NULL COMMENT '计划编号',
  `counts` int(10) NOT NULL COMMENT '数量',
  `createdate` datetime NOT NULL COMMENT '创建时间',
  `others` text COMMENT '备注',
  PRIMARY KEY (`collect_no`),
  KEY `pk_name` (`machine_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='生产信息汇总表' AUTO_INCREMENT=68 ;

--
-- 转存表中的数据 `mes_collect`
--

INSERT INTO `mes_collect` (`collect_no`, `correlation_no`, `machine_no`, `products_no`, `staff_no`, `plans_no`, `counts`, `createdate`, `others`) VALUES
(66, 65, 1, 1114, 0, 0, 43, '2016-08-01 02:00:00', NULL),
(65, 22, 3, 3, 0, 0, 57, '2016-07-26 03:00:00', NULL),
(64, 65, 1, 1114, 0, 0, 24, '2016-07-26 03:00:00', NULL),
(63, 65, 1, 1114, 0, 0, 6, '2016-07-26 02:00:00', NULL),
(52, 43, 1, 2, 0, 0, 200, '2016-07-25 08:00:00', NULL),
(53, 44, 1, 2, 0, 0, 0, '2016-07-25 08:00:00', NULL),
(54, 45, 1, 2, 0, 0, 200, '2016-07-25 08:00:00', NULL),
(55, 46, 1, 2, 0, 0, 0, '2016-07-25 08:00:00', NULL),
(56, 47, 1, 2, 0, 0, 0, '2016-07-25 08:00:00', NULL),
(57, 48, 1, 1114, 0, 0, 12, '2016-07-26 02:00:00', NULL),
(58, 49, 1, 1114, 0, 0, 1, '2016-07-26 02:00:00', NULL),
(41, 36, 1, 1, 0, 0, 0, '2016-07-25 08:00:00', NULL),
(42, 23, 2, 2, 0, 0, 55, '2016-07-25 08:00:00', NULL),
(43, 22, 3, 3, 0, 0, 52, '2016-07-25 08:00:00', NULL),
(44, 37, 1, 1, 0, 0, 50, '2016-07-25 08:00:00', NULL),
(62, 65, 1, 1114, 0, 0, 4, '2016-07-26 02:00:00', NULL),
(61, 64, 1, 1114, 0, 0, 5, '2016-07-26 02:00:00', NULL),
(60, 55, 1, 1114, 0, 0, 0, '2016-07-26 02:00:00', NULL),
(59, 50, 1, 1114, 0, 0, 6, '2016-07-26 02:00:00', NULL),
(67, 67, 1, 1114, 0, 0, 0, '2016-08-01 03:00:00', NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_correlation`
--

CREATE TABLE IF NOT EXISTS `mes_correlation` (
  `correlation_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '关联编号',
  `staff_no` int(10) DEFAULT NULL COMMENT '员工编号',
  `machine_no` int(10) NOT NULL COMMENT '机器编号',
  `products_no` int(10) NOT NULL COMMENT '产品编号',
  `createdate` datetime NOT NULL COMMENT '创建时间',
  `others` text COMMENT '备注',
  PRIMARY KEY (`correlation_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='关联表' AUTO_INCREMENT=69 ;

--
-- 转存表中的数据 `mes_correlation`
--

INSERT INTO `mes_correlation` (`correlation_no`, `staff_no`, `machine_no`, `products_no`, `createdate`, `others`) VALUES
(59, NULL, 1, 1114, '2016-07-26 14:32:56', NULL),
(49, NULL, 1, 1114, '2016-07-26 14:25:15', NULL),
(50, NULL, 1, 1114, '2016-07-26 14:29:39', NULL),
(51, NULL, 1, 1114, '2016-07-26 14:31:40', NULL),
(52, NULL, 1, 1114, '2016-07-26 14:31:43', NULL),
(53, NULL, 1, 1114, '2016-07-26 14:31:46', NULL),
(54, NULL, 1, 1114, '2016-07-26 14:31:49', NULL),
(55, NULL, 1, 1114, '2016-07-26 14:31:52', NULL),
(56, NULL, 1, 1114, '2016-07-26 14:32:47', NULL),
(57, NULL, 1, 1114, '2016-07-26 14:32:50', NULL),
(58, NULL, 1, 1114, '2016-07-26 14:32:53', NULL),
(20, NULL, 1, 1, '2016-07-25 19:16:30', NULL),
(60, NULL, 1, 1114, '2016-07-26 14:32:59', NULL),
(22, NULL, 3, 3, '2016-07-21 15:13:28', NULL),
(23, NULL, 2, 2, '2016-07-21 15:13:28', NULL),
(61, NULL, 1, 1114, '2016-07-26 14:33:02', NULL),
(48, NULL, 1, 1114, '2016-07-26 14:24:22', NULL),
(47, NULL, 1, 2, '2016-07-25 20:21:33', NULL),
(62, NULL, 1, 1114, '2016-07-26 14:33:05', NULL),
(63, NULL, 1, 1114, '2016-07-26 14:33:08', NULL),
(64, NULL, 1, 1114, '2016-07-26 14:33:19', NULL),
(65, NULL, 1, 1114, '2016-07-26 14:36:49', NULL),
(66, NULL, 1, 1114, '2016-08-01 14:31:50', NULL),
(67, NULL, 1, 1114, '2016-08-01 14:31:53', NULL),
(68, NULL, 1, 1114, '2016-08-02 10:59:47', NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_inferior`
--

CREATE TABLE IF NOT EXISTS `mes_inferior` (
  `inferior_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '不良编号',
  `products_no` int(10) NOT NULL COMMENT '产品编号',
  `staff_no` int(10) DEFAULT NULL COMMENT '员工编号',
  `machine_no` int(10) NOT NULL COMMENT '机器编号',
  `count` int(10) NOT NULL COMMENT '数量',
  `createdate` datetime NOT NULL COMMENT '创建时间',
  `others` text COMMENT '备注',
  PRIMARY KEY (`inferior_no`),
  KEY `FK_ID` (`products_no`),
  KEY `FK_ID1` (`staff_no`),
  KEY `FK_ID2` (`machine_no`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='不良产品信息' AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `mes_machine`
--

CREATE TABLE IF NOT EXISTS `mes_machine` (
  `machine_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '机器编号',
  `machinename` varchar(10) DEFAULT '机械手' COMMENT '机器名称',
  `machinetype` varchar(10) DEFAULT NULL COMMENT '机器类型',
  `others` text COMMENT '备注',
  PRIMARY KEY (`machine_no`),
  UNIQUE KEY `machineno` (`machine_no`),
  UNIQUE KEY `machinename` (`machinename`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='机器信息表' AUTO_INCREMENT=5 ;

--
-- 转存表中的数据 `mes_machine`
--

INSERT INTO `mes_machine` (`machine_no`, `machinename`, `machinetype`, `others`) VALUES
(1, '注塑机', NULL, '测试'),
(2, '机械手', 'e', NULL),
(3, '蜘蛛手', 'd', NULL),
(4, 'AGV', NULL, '测试2');

-- --------------------------------------------------------

--
-- 表的结构 `mes_machinetime`
--

CREATE TABLE IF NOT EXISTS `mes_machinetime` (
  `time_no` int(10) NOT NULL AUTO_INCREMENT,
  `machine_no` int(10) NOT NULL,
  `workingtime` int(15) NOT NULL,
  `workinghour` double(10,2) NOT NULL,
  `createdate` date NOT NULL,
  `others` text,
  PRIMARY KEY (`time_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='机器工作时间表' AUTO_INCREMENT=54 ;

--
-- 转存表中的数据 `mes_machinetime`
--

INSERT INTO `mes_machinetime` (`time_no`, `machine_no`, `workingtime`, `workinghour`, `createdate`, `others`) VALUES
(1, 1, 155, 2.58, '2016-07-24', NULL),
(2, 2, 180, 3.00, '2016-07-24', NULL),
(3, 3, 60, 1.00, '2016-07-23', NULL),
(53, 3, 0, 0.00, '2016-08-02', NULL),
(52, 2, 0, 0.00, '2016-08-02', NULL),
(51, 1, 0, 0.00, '2016-08-02', NULL),
(21, 3, 35, 0.58, '2016-07-25', NULL),
(20, 2, 35, 0.58, '2016-07-25', NULL),
(50, 3, 5, 0.08, '2016-08-01', NULL),
(49, 2, 5, 0.08, '2016-08-01', NULL),
(48, 1, 5, 0.08, '2016-08-01', NULL),
(47, 3, 15, 0.25, '2016-07-26', NULL),
(46, 2, 10, 0.17, '2016-07-26', NULL),
(45, 1, 25, 0.42, '2016-07-26', NULL),
(44, 1, 0, 0.00, '2016-07-25', NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_message`
--

CREATE TABLE IF NOT EXISTS `mes_message` (
  `message_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '报文编号',
  `detail` varchar(15) NOT NULL COMMENT '报文内容',
  `createdate` datetime NOT NULL COMMENT '创建日期',
  `others` text COMMENT '备注',
  PRIMARY KEY (`message_no`),
  UNIQUE KEY `message_no` (`message_no`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='报文信息表' AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `mes_plans`
--

CREATE TABLE IF NOT EXISTS `mes_plans` (
  `plans_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '计划编号',
  `products_no` int(10) NOT NULL COMMENT '产品编号',
  `count` int(10) NOT NULL COMMENT '产品计划数',
  `createdate` datetime NOT NULL COMMENT '创建日期',
  `others` text COMMENT '备注',
  PRIMARY KEY (`plans_no`),
  KEY `products_no` (`products_no`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='生产计划表' AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `mes_products`
--

CREATE TABLE IF NOT EXISTS `mes_products` (
  `products_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '产品编号',
  `pcard_no` int(10) NOT NULL DEFAULT '0' COMMENT '产品卡编号',
  `productsname` varchar(10) NOT NULL COMMENT '产品名',
  `hole` int(3) NOT NULL DEFAULT '1' COMMENT '穴数',
  `others` text COMMENT '备注',
  PRIMARY KEY (`products_no`),
  UNIQUE KEY `products_no` (`products_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='产品信息表' AUTO_INCREMENT=1115 ;

--
-- 转存表中的数据 `mes_products`
--

INSERT INTO `mes_products` (`products_no`, `pcard_no`, `productsname`, `hole`, `others`) VALUES
(1, 1, '塑料灯管', 2, ''),
(2, 4, '测试', 4, ''),
(3, 0, '汽车零件1', 1, NULL),
(4, 0, '汽车2', 3, NULL),
(1114, 0, 'asd', 1, NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_situation`
--

CREATE TABLE IF NOT EXISTS `mes_situation` (
  `situation_no` int(10) NOT NULL AUTO_INCREMENT,
  `explain` text,
  `others` text,
  PRIMARY KEY (`situation_no`),
  UNIQUE KEY `situation_no` (`situation_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='机器状态表' AUTO_INCREMENT=10 ;

--
-- 转存表中的数据 `mes_situation`
--

INSERT INTO `mes_situation` (`situation_no`, `explain`, `others`) VALUES
(2, '远程停止', NULL),
(1, '远程运行', NULL),
(3, '外部上使能', NULL),
(4, '远程复位', NULL),
(5, '运行中', NULL),
(6, '伺服已接通', NULL),
(7, '报警输出', '机器出现问题'),
(8, '地线0V', NULL),
(9, '输出+24V', NULL);

-- --------------------------------------------------------

--
-- 表的结构 `mes_staff`
--

CREATE TABLE IF NOT EXISTS `mes_staff` (
  `staff_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '员工编号',
  `staffname` varchar(10) DEFAULT NULL COMMENT '员工名',
  `others` text COMMENT '备注',
  PRIMARY KEY (`staff_no`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='员工表' AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `mes_stoppage`
--

CREATE TABLE IF NOT EXISTS `mes_stoppage` (
  `stoppage_no` int(10) NOT NULL AUTO_INCREMENT COMMENT '故障编号',
  `machine_no` int(10) NOT NULL COMMENT '机器编号',
  `situation_no` int(10) NOT NULL COMMENT '不良信息号',
  `explain` text NOT NULL COMMENT '机器状态',
  `createdate` datetime NOT NULL COMMENT '创建时间',
  `others` text COMMENT '备注',
  PRIMARY KEY (`stoppage_no`),
  KEY `FK_ID` (`machine_no`),
  KEY `FK_ID1` (`situation_no`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COMMENT='故障信息表' AUTO_INCREMENT=11 ;

--
-- 转存表中的数据 `mes_stoppage`
--

INSERT INTO `mes_stoppage` (`stoppage_no`, `machine_no`, `situation_no`, `explain`, `createdate`, `others`) VALUES
(2, 1, 2, '3', '0000-00-00 00:00:00', '5'),
(3, 1, 2, 'stop', '0000-00-00 00:00:00', 'hh'),
(4, 1, 2, 'stop', '0000-00-00 00:00:00', 'hh'),
(6, 0, 2, 's', '2016-07-21 16:27:07', NULL),
(7, 0, 2, '中文', '2016-07-21 16:27:07', NULL),
(8, 0, 2, 'stop', '2016-07-21 17:02:55', NULL),
(9, 1234, 2, 'stop', '2016-07-21 17:05:19', NULL),
(10, 1234, 2, '停止', '2016-07-21 17:06:13', NULL);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
