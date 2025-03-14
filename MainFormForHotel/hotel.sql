/*
 Navicat Premium Data Transfer

 Source Server         : Mysql 9
 Source Server Type    : MySQL
 Source Server Version : 90100
 Source Host           : localhost:3309
 Source Schema         : hotel

 Target Server Type    : MySQL
 Target Server Version : 90100
 File Encoding         : 65001

 Date: 14/03/2025 20:44:51
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for guestcheck
-- ----------------------------
DROP TABLE IF EXISTS `guestcheck`;
CREATE TABLE `guestcheck`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `guestcheckid` bigint NOT NULL,
  `busdate` datetime NULL DEFAULT NULL,
  `locationid` bigint NULL DEFAULT 2041,
  `revenuecenterid` bigint NULL DEFAULT 12950,
  `checkNum` bigint NULL DEFAULT NULL,
  `openDateTime` datetime NULL DEFAULT NULL,
  `checkTotal` decimal(60, 6) NULL DEFAULT 0.000000,
  `numItems` bigint NULL DEFAULT 0,
  `firstName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `lastName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `is_download` int NULL DEFAULT 0 COMMENT '0是未下载 ，其余为已下载次数',
  `downoad_datetime` datetime NULL DEFAULT NULL COMMENT '上次下载的时间',
  `getdatadate` varchar(32) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT '记录日期2024-07-01',
  `insert_dt` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1173 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for guestcheckdetails
-- ----------------------------
DROP TABLE IF EXISTS `guestcheckdetails`;
CREATE TABLE `guestcheckdetails`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `transTime` datetime NULL DEFAULT NULL,
  `serviceRoundNum` bigint NULL DEFAULT NULL,
  `lineNum` bigint NULL DEFAULT NULL,
  `guestCheckLineItemID` bigint NULL DEFAULT NULL,
  `detailType` int NULL DEFAULT NULL,
  `itemName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `itemName2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL COMMENT 'reference infor name',
  `itemchname` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `rvcName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `firstName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `lastName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `reasonVoidText` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `returnText` varchar(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `recordID` bigint NULL DEFAULT NULL,
  `salesTotal` decimal(20, 6) NULL DEFAULT NULL,
  `salesCount` int NULL DEFAULT NULL,
  `salesCountDivisor` varchar(32) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `locationID` bigint NULL DEFAULT 2041,
  `doNotShow` int NULL DEFAULT NULL,
  `guestCheckID` bigint NULL DEFAULT NULL,
  `organizationID` bigint NULL DEFAULT 10260,
  `checkNum` bigint NULL DEFAULT NULL,
  `insert_dt` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 692 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for guestcheckdetailssumrow
-- ----------------------------
DROP TABLE IF EXISTS `guestcheckdetailssumrow`;
CREATE TABLE `guestcheckdetailssumrow`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `organizationID` bigint NULL DEFAULT NULL,
  `checkNum` bigint NULL DEFAULT NULL,
  `tableRef` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `openDatetime` datetime NULL DEFAULT NULL,
  `duration` decimal(19, 6) NULL DEFAULT NULL,
  `numGuests` int NULL DEFAULT NULL,
  `checkRef` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `locName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `rvcName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `otName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `firstName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `lastName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL DEFAULT NULL,
  `guestCheckID` bigint NULL DEFAULT NULL,
  `locationID` int NULL DEFAULT 2041,
  `insert_dt` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
