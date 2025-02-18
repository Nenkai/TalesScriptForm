using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm;

public enum ScfomInstructionType : byte
{
    // Pops one int value (syscall number as lower 24 bits) + num of arguments.
    // -1 may be special.
    SCFOM_INST_SYSCALL = 0,

    SCFOM_INST_CALL = 1,

    // InstPtr = StartCodePtr + (uint)&Data[1]
    SCFOM_INST_JUMP = 2,

    // Pop stack value
    // if value == Data[0], skip
    // otherwise, InstPtr = StartCodePtr + (uint)&Data[1]
    SCFOM_INST_JUMP_IF = 3,

    SCFOM_INST_EXIT = 4,

    // Implicit or explicit?
    // returns a modified value of the top stack value, based on specified type
    SCFOM_INST_CAST = 5,

    // *StackPtr = value
    // NOTE: data size must be 2 (byte), 3 (short), 5 (int), 9 (long).
    SCFOM_INST_PUSH_INT = 6,

    // StackPtr -= 8
    // *Stackptr = *ConstantTable[index] + offset
    // NOTE: data size must be 3 (byte), 4 (short), 6 (int).
    SCFOM_INST_PUSH_CONST = 7,

    // StackPtr -= 8
    // *StackPtr = Registers[index] - if instruction size is 1, index = 0
    SCFOM_INST_PUSH_FROM_REGISTER = 8,

    // *StackPtr = Registers[index] - if instruction size is 1, index = 0
    // StackPtr += 8
    SCFOM_INST_ASSIGN_POP_TO_REGISTER = 9,

    // *StackPtr += value
    // NOTE: value can be negative, += is pop, -= is push
    SCFOM_INST_STACK_SEEK = 10,

    // Grabs 3 bytes, following bits for each byte:
    // 6 bit: calcFlag, 5 bit = lhs type, 5 bit = rhs type
    // calcFlag is used as operation type, no more than 36
    SCFOM_INST_CALC = 11,

    // Indexes into a big table, which returns a modified value of the top stack value
    // Based on specified types
    // Implcit or explicit?
    // Mostly used when passing a NULL value.
    SCFOM_INST_CAST2 = 12,
};

public enum ScfomConstantType : byte
{
    // [0] = GlobalCodeOffsetPtr
    // [1] = DataPtr
    // [2] = StringTablePtr
    // [3] = StackPtr (current)
    // [4] = StackStartPtr, we go backwards to grow
    // [5] = unk5 table
    // [6] = unk6 (is this even an offset?) table
    // [7] = unk7 table

    CONST_CODE = 0,
    CONST_DATA = 1,
    CONST_STRINGS = 2,
    CONST_STACK_PTR = 3,
    CONST_STACK_START = 4,
    CONST_FIELD_UNK5 = 5, // Unk1
    CONST_FIELD_UNK6 = 6, // Unk2
    CONST_FIELD_UNK7 = 7, // Unk3
};

public enum ScfomCalcOperator : byte
{
    OP_ADD = 0,
    OP_SUB = 1,
    OP_MUL = 2,
    OP_DIV = 3,
    OP_MOD = 4,
    OP_BITWISE_AND = 5,
    OP_BITWISE_OR = 6,
    OP_BITWISE_XOR = 7,
    OP_LOGICAL_RIGHT_SHIFT = 8,
    OP_LOGICAL_LEFT_SHIFT = 9,
    OP_ARITHMETIC_RIGHT_SHIFT = 10,
    OP_ARITHMETIC_LEFT_SHIFT = 11, // same OP_LOGICAL_LEFT_SHIFT?, no rotating
    OP_UNARY_MINUS = 12,
    OP_UNARY_BITWISE_NOT = 13,
    OP_UNARY_LOGICAL_NOT = 14,
    OP_ADD_UNSIGNED = 15,
    OP_BINARY_ASSIGN_PLUS = 16, // +=
    OP_EQ = 17,
    OP_NEQ = 18,
    OP_GREATER_EQ_TO = 19,
    OP_LESSER_EQ_TO = 20,
    OP_GREATER_THAN = 21,
    OP_LESSER_THAN = 22,
    OP_ASSIGN = 23,
    OP_COPY_ARRAY = 24, // Copy array?

    // Signed?
    OP_ADD_UNK = 25, // Plus again?
    OP_SUB_UNK = 26, // Minus again?
    OP_MUL_UNK = 27, // Mult again?
    OP_DIV_UNK = 28, // Div again?
    OP_MOD_UNK = 29, // Mod again?
    OP_AND_UNK = 30, // And again?
    OP_BITWISE_OR_UNK = 31, // Or again?
    OP_BITWISE_XOR_UNK = 32, // Xor again?
    OP_LOGICAL_RIGHT_SHIFT_UNK = 33, // Right shift?
    OP_LOGICAL_LEFT_SHIFT_UNK = 34, // Left shift?
    OP_ARITHMETIC_RIGHT_SHIFT_UNK = 35, // Right shift 2?
    OP_ARITHMETIC_LEFT_SHIFT_UNK = 36, // Left shift 2?

    OP_COMPARE = 37, // Left < right = -1, right > left = 1, equal = 0
    OP_38 = 38,
}

// syscall 1: wait

// Scf v3
public enum ScfomDataType
{
    // & 1 = unsigned
    // & 2 = float/double
    TYPE_SIZET, // This is 4 in ps2/32bit games
    TYPE_USIZET, // This is 4 in ps2/32bit games
    TYPE_FSIZET,
    TYPE_0_3,

    TYPE_S8,
    TYPE_U8,
    TYPE_1_6,
    TYPE_0_7,

    TYPE_2_S16,
    TYPE_2_U16,
    TYPE_2_10,
    TYPE_0_11,

    TYPE_S32,
    TYPE_U32,
    TYPE_F32,
    TYPE_0_15,

    // V3 only
    TYPE_S64,
    TYPE_U64,
    TYPE_F64,
    TYPE_0_19,

    TYPE_0_20,
    TYPE_0_21,
    TYPE_0_22,
    TYPE_0_23,
}