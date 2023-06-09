#-*- coding: UTF-8 -*- 
##
# @file:   excel_parser.py
# @author:  Triniti Interactive Limited
# Copyright (c) Triniti Interactive Limited All rights reserved.
#
# This code is licensed under the MIT License (MIT).
# THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
# ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
# IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
# PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

# after .proto and autogenerated_pb2.py created.
# reflect all attributes from autogenerated_pb2.py and fill the data.
# Create data.bin file

import xlrd
import os
import sys
from Utils import *


class WorkbookParser:
    def __init__(self, excel_file_path, protos_python_path):
        self._excel_file_path = excel_file_path
        (excel_file_dir, excel_file_name_withext) = os.path.split(excel_file_path)
        (self._excel_file_name, excel_file_name_ext) = os.path.splitext(
            excel_file_name_withext)
        self._module_name = self._excel_file_name + "_pb2"
        # old is bin
        self._output_file_name = self._excel_file_name + ".bin"
        # load excel file
        try:
            self._workbook = xlrd.open_workbook(excel_file_path)
        except Exception as e:
            logError("open xls file(%s) failed! error:%s" %
                     (excel_file_path, e))
            raise
        # load python file
        try:
            # sys.path.append(os.getcwd())
            sys.path.append(protos_python_path)
            exec('from ' + self._module_name + ' import *')
            self._module = sys.modules[self._module_name]
        except Exception as e:
            logError("load module(%s) failed! error:%s " %
                     (self._module_name, e))
            raise
        self._workbook_data_root = getattr(
            self._module, self._excel_file_name + '_Data')()
        self._parse()

    def _get_data_readable(self):
        return str(self._workbook_data_root)

    def _get_data_binaray(self):
        return self._workbook_data_root.SerializeToString()

    def _parse(self):
        try:
            for sheet in self._workbook.sheets():
                # sheet_item_class = getattr(self._module, sheet.name)()
                # item_obj = sheet_item_class()
                SheetParser(self._workbook_data_root, sheet).parse()
        except Exception as e:
            logError("open sheet file(%s) failed! errror:%s" %
                     (self._excel_file_path, e))
            raise

    def serialize(self, temp_proto_data_path, data_out):
        data = self._get_data_binaray()
        pb_file = data_out if data_out is not None else temp_proto_data_path
        file_path = pb_file + "/" + self._output_file_name
        file = open(file_path, 'wb+')
        file.write(data)
        file.close()
        log("exported protobuff data to :%s" % file_path)
        # data = self._get_data_readable()
        # file_name = temp_proto_data_path + "/" + self._excel_file_name + ".txt"
        # file = open(file_name, 'w+')
        # file.write(data)
        # file.close()


DATA_ROW_START = 4
# assert first column must be the id col
FIELD_ORDER_INDEX = 0
FIELD_NAME_ROW = 1
FIELD_TYPE_ROW = 2
FIELD_COMMENT_ROW = 3


class SheetParser:
    def __init__(self, data_root, sheet):
        self._sheet = sheet
        self._item_map = getattr(data_root, self._sheet.name+"_items")
        self._row_count = len(self._sheet.col_values(0))
        self._col_count = len(self._sheet.row_values(0))

    def _get_sheet_struct_name(self):
        return self._sheet.name

    def parse(self):
        log("parse sheet %s" % self._sheet.name)
        for cur_row in range(DATA_ROW_START, self._row_count):
            value = self._sheet.cell_value(cur_row, 0)
            if value is None or value == '':
                continue
            item_id = int(value)
            if item_id in self._item_map:
                error = "!!!sheet:%s id:%s duplicated id!!!" % (
                    self._sheet.name, item_id)
                logError(error)
                raise RuntimeError(error)
            item = self._item_map.get_or_create(item_id)
            self._parse_row(item, cur_row)
            # self._item_map[item_id] = item
            # item = self._item_map.add()
            # item_id = self._parse_row(item, cur_row)
        # print(str(self._item_map))
        return self

    def _parse_row(self, item, cur_row):
        # id should always at the first column
        for column_index in range(0, self._col_count):
            field_name = self._sheet.cell_value(FIELD_NAME_ROW, column_index)
            if field_name.startswith('#') or len(field_name) <= 0:
                continue
            field_type = self._sheet.cell_value(FIELD_TYPE_ROW, column_index)
            if str(field_type).endswith(' '):
                error = "!!!sheet:%s column:%s type:%s end with blank!!!" % (
                    self._sheet.name, field_name, field_type)
                logError(error)
                raise RuntimeError(error)
            field_value = self._sheet.cell_value(cur_row, column_index)
            self._set_item_field(item, field_name, field_type, field_value)
            # print(field_name, field_strong_value)

    def _set_item_field(self, item, field_name, field_type, field_value):
        try:
            is_repeated = False
            if field_type.startswith("repeated"):
                field_type = field_type.split(' ')[1]
                is_repeated = True
            if is_repeated:
                # data field is not a array
                if str(field_value).find('|') == -1:
                    field_strong_value = self._get_field_strong_value_single(
                        field_name, field_type, field_value)
                    if field_strong_value is not None:
                        item.__getattribute__(field_name).append(
                            field_strong_value)
                else:
                    if str(field_value).find(';') != -1:
                        field_value = str(field_value).replace(';', '|')
                    splited_values = field_value.split('|')
                    for splited_value in splited_values:
                        field_strong_value = self._get_field_strong_value_single(
                            field_name, field_type, splited_value)
                        if field_strong_value is not None:
                            item.__getattribute__(field_name).append(
                                field_strong_value)
            else:
                field_strong_value = self._get_field_strong_value_single(
                    field_name, field_type, field_value)
                if field_strong_value is not None:
                    item.__setattr__(field_name, field_strong_value)
        except Exception as e:
            print("please check it, maybe type is wrong. sheet:%s column:%s e:%s" % (
                self._sheet.name, field_name, e))
            raise

    def _get_field_strong_value_single(self, field_name, field_type, field_value):
        try:
            if field_type == "int32" or field_type == "int64"\
                    or field_type == "uint32" or field_type == "uint64"\
                    or field_type == "sint32" or field_type == "sint64"\
                    or field_type == "fixed32" or field_type == "fixed64"\
                    or field_type == "sfixed32" or field_type == "sfixed64":
                        if len(str(field_value).strip()) <= 0:
                            return None
                        else:
                            return int(field_value)
            elif field_type == "bool":
                return field_value == 1
            elif field_type == "double" or field_type == "float":
                    if len(str(field_value).strip()) <= 0:
                        return None
                    else:
                        return float(field_value)
            elif field_type == "string":
                if isinstance(field_value, float) and int(field_value) == field_value:
                    field_value = int(field_value)
                field_value = field_value
                if len(str(field_value)) <= 0:
                    return None
                else:
                    return str(field_value)
            elif field_type == "bin":
                field_value = field_value.encode('utf-8')
                if len(field_value) <= 0:
                    return None
                else:
                    return field_value
            else:
                return None
        except Exception as e:
            logError("please check it, maybe type is wrong. sheet:%s column:%s e:%s" % (
                self._sheet.name, field_name, e))
            raise
