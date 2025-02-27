﻿using System;
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

// Tales of Rebirth
// Symbols are conveniently available in a overlay, at least in the PSP version
// look for 'hostfunc'
public enum ScfomSyscall_V20100 // Tales of Rebirth
{
    printf = 0,
    wait = 1,
    exec_local = 2,
    exec_global = 3,
    destroy_id = 4,
    rand = 5,
    get_bit_shift = 6,
    get_bit_count = 7,
    memcpy = 8,
    create_texture = 9,
    create_effect = 10,
    create_hit = 11,
    destroy_id_2 = 12, // Original = 'destroy_id', but conflicts with syscall 4
    get_palette = 13,
    set_palette = 14,
    set_chara_palette = 15,
    get_chara_param = 16,
    set_chara_param = 17,
    get_hit_param = 18,
    set_hit_param = 19,
    get_field_param = 20,
    set_field_param = 21,
    is_id = 22,
    create_item = 23,
    get_macro_result = 24,
    set_field_color = 25,
    get_position_id = 26,
    get_position_2d = 27,
    get_effect_param = 28,
    get_chara_param_f = 29,
    set_chara_param_f = 30,
    rot_vector = 31,
    check_boot_item = 32,
    set_field_observe = 33,
    set_pause = 34,
    enable_pause = 35,
    open_menu = 36,
    sprintf = 37,
    get_distance = 38,
    addmove_id = 39,
    get_effect_id = 40,
    set_effect_param = 41,
    blow_object = 42,
    fade = 43,
    vibration = 44,
    make_vector = 45,
    blare = 46,
    sinf = 47,
    cosf = 48,
    get_direction = 49,
    sound = 50,
    music = 51,
    message = 52,
    set_person = 53,
    move_position = 54,
    set_direction = 55,
    set_camera = 56,
    get_field_param_i = 57,
    check = 58,
    exec_remote = 59,
    set_game = 60,
    set_scenario = 61,
    check_wait = 62,
    enable_skip = 63,
    strcpy = 64,
    font2sjis = 65,
    set_flag = 66,
    get_flag = 67,
    set_discovery = 68,
    get_discovery_texinfo = 69,
    register_callback = 70,
    set_forcecube = 71,
    set_itemset = 72,
    reset_field = 73,
    get_free_pos = 74,
    get_item_count = 75,
    start_gumipuyo = 76,
    finish_gumipuyo = 77,
    get_mission_title = 78,
    add_score = 79,
    add_count_gumipuyo = 80,
    get_count_gumipuyo = 81,
    set_highscore = 82,
    get_highscore = 83,
    get_star = 84,
    get_add_title_all_str = 85,
    add_title = 86,
    check_title = 87,
    vibrate_pad = 88,
    voice = 89,
    get_distance_2d = 90,
    set_move_infield = 91,
    print_debug = 92,
    tomoicheck = 93,
}

public enum ScfomSyscall_V30100  // Tales of Vesperia, found in executable of DE
{
    print_debug = 1,
    set_person = 2,
    change_map = 3,
    set_param = 4,
    set_motion = 5,
    set_pos = 6,
    set_model = 7,
    get_global = 8,
    get_pad_dat = 9,
    get_pad_rep = 10,
    get_pad_new = 11,
    create_effect = 12,
    rand = 13,
    init_effect_param = 14,
    suspend = 15,
    rand_f = 16,
    get_effect_arg = 17,
    printf_ = 18,
    sprintf_ = 19,
    printf_debug_ = 20,
    init_effect_calc = 21,
    copy_effect_arg = 23,
    set_effect_arg = 24,
    create_thread_ = 25,
    get_effect_param = 26,
    is_effect = 27,
    set_scope = 28,
    show_debug = 29,
    set_framerate = 30,
    stop_motion = 31,
    set_map = 32,
    delete_map = 33,
    load_map = 34,
    set_map_exclusive = 35,
    delete_person = 36,
    get_param = 37,
    fadeout = 38,
    fadein = 39,
    set_env = 40,
    get_env = 41,
    enemy_encount = 42,
    get_dir_p2p = 43,
    set_scope_actinfo = 44,
    set_field_camera_angle = 45,
    set_weather = 46,
    get_weather = 47,
    is_gamestate = 48,
    set_gadget = 49,
    delete_gadget = 50,
    get_pos = 51,
    set_motion_define = 52,
    move_pos = 53,
    set_motion_script = 55,
    enable_motion_script = 56,
    start_demo = 57,
    finish_demo = 58,
    btl_order_effect = 59,
    btl_is_ready_effect = 60,
    btl_generate_effect = 61,
    btl_generate_effect_exec_script = 62,
    get_effect_time = 63,
    destroy_effect = 64,
    set_lookat = 65,
    set_lookat_define = 66,
    btl_start_magic_arts = 67,
    btl_end_magic_arts = 68,
    window_move = 69,
    window_time = 70,
    message_window = 71,
    info_window = 72,
    select_window = 73,
    btl_get_chara_num = 74,
    btl_get_chara_no = 75,
    btl_is_chara = 76,
    btl_reset_chara_hit_list = 77,
    get_select = 78,
    get_pad_bit = 79,
    set_ladder = 80,
    get_string = 81,
    set_string = 82,
    get_effect_totaltime = 83,
    sinf = 84,
    cosf = 85,
    tanf = 86,
    replace_effect_texture = 87,
    register_drag_object = 88,
    btl_destroy_effect = 89,
    get_encount_enemy_num = 96,
    set_flag = 97,
    get_flag = 98,
    set_camera = 99,
    set_camera_data = 100,
    set_camera_from = 101,
    set_camera_at = 102,
    set_camera_target = 103,
    set_camera_shifttime = 104,
    get_encount_enemy_id = 105,
    register_chara_encount = 106,
    register_gadget_encount = 107,
    learn_skill = 108,
    btl_set_time_stop = 109,
    btl_is_time_stop = 110,
    set_door = 111,
    set_camera_fov = 112,
    btl_get_chara_num_from_party_no = 113,
    btl_get_chara_no_from_party_no = 114,
    get_encount_enemy_group_num = 115,
    move_dir = 116,
    get_encount_enemy_group_id = 117,
    btl_is_play_effect = 118,
    get_current_map = 119,
    set_motion_define_ex = 120,
    set_scope_circle = 121,
    set_scope_linebox = 122,
    set_scope_direction = 123,
    close_window = 124,
    get_window_state = 125,
    is_window = 126,
    set_wall = 127,
    delete_wall = 128,
    get_dir_v2v = 129,
    get_dir_v2v_direct = 130,
    get_gadget_arg = 131,
    is_gadget = 132,
    finish_demo_force = 133,
    is_person = 135,
    get_distance_p2p = 136,
    get_distance_v2v = 137,
    get_distance_v2v_direct = 138,
    is_learn_magic_arts = 139,
    delete_scope = 140,
    set_camera_from_relative = 141,
    register_conversation = 142,
    unregister_conversation = 143,
    start_scene = 144,
    btl_request = 145,
    btl_set_comment = 146,
    btl_set_param_f = 147,
    btl_get_param_f = 148,
    btl_check_flag = 149,
    btl_assert = 150,
    btl_warning = 151,
    is_learn_skill = 152,
    is_set_skill = 153,
    get_encount_group_num = 154,
    get_encount_group_id = 155,
    btl_set_magic_arts_param = 156,
    btl_set_magic_arts_param_f = 157,
    btl_get_magic_arts_param = 158,
    btl_get_magic_arts_param_f = 159,
    btl_get_enable_use_magic_arts_list_num = 160,
    btl_get_enable_use_magic_arts_list = 161,
    btl_check_use_magic_arts = 162,
    btl_check_chain_magic_arts = 163,
    btl_check_range_magic_arts = 164,
    sqrtf = 165,
    abs = 166,
    fabs = 167,
    set_camera_motion_file = 168,
    set_camera_motion = 169,
    btl_start_degenerate_bg = 170,
    set_balloon = 171,
    delete_balloon = 172,
    set_balloon_scale = 173,
    is_balloon = 174,
    set_kk = 175,
    delete_kk = 176,
    get_first_name = 177,
    get_last_name = 178,
    get_pet_name = 179,
    get_full_name = 180,
    set_balloon_direct = 181,
    reset_balloon = 182,
    delete_treasure = 184,
    set_map_layer_param = 185,
    get_map_layer_param = 186,
    set_scope_height = 187,
    mtx_rotation_x = 188,
    mtx_rotation_y = 189,
    mtx_rotation_z = 190,
    mtx_scaling = 191,
    mtx_translation = 192,
    mtx_multiply = 193,
    mtx_identity = 194,
    vec_add = 195,
    vec_cross = 196,
    vec_dot = 197,
    vec_length = 198,
    vec_transform_v3 = 199,
    memset = 200,
    memcpy = 201,
    get_motion_define_param = 202,
    item_setcount = 203,
    item_addcount = 204,
    item_getcount = 205,
    set_motion_ex = 206,
    item_getname = 207,
    register_ai = 208,
    unregister_ai = 209,
    is_enable_ai = 210,
    set_enable_ai = 211,
    set_enable_ai_all = 212,
    party_in = 213,
    party_out = 214,
    crossfade = 215,
    set_place_name = 216,
    get_place_name = 217,
    get_last_error = 218,
    set_move_circle = 219,
    set_move_rect = 220,
    set_move_path = 221,
    order_command_ai = 222,
    goto_state_ai = 223,
    get_param_ai = 224,
    set_param_ai = 225,
    set_color = 226,
    is_state_ai = 227,
    register_footmark = 228,
    unregister_footmark = 229,
    set_effect_scene_time = 230,
    delete_sorcererring = 231,
    strcpy = 232,
    strcmp = 233,
    strlen = 234,
    strcat = 235,
    music = 236,
    music_vol = 237,
    set_debug_comment_ai = 239,
    register_alias_motion_of_ai = 243,
    unregister_alias_motion_of_ai = 244,
    get_alias_motion_of_ai = 245,
    set_balloon_move = 246,
    get_camera_data = 247,
    force_update_dynamic_bone = 248,
    set_object_ai = 249,
    delete_object_ai = 250,
    set_object_area_circle_ai = 251,
    set_object_area_point_ai = 252,
    get_object_param_ai = 253,
    set_object_param_ai = 254,
    set_balloon_loop_frame = 255,
    set_effect_scene_motion = 256,
    scenario_window = 257,
    set_req_value_c = 258,
    set_req_value_s = 259,
    set_req_value_d = 260,
    set_req_value_u = 261,
    set_req_value_f = 262,
    get_free_id = 263,
    register_command_ai = 265,
    unregister_command_ai = 266,
    reset_command_ai = 267,
    get_command_count_ai = 268,
    get_map_object_param = 269,
    get_map_object_pos = 270,
    reset_fs_se = 271,
    set_fs_se_default = 272,
    set_fs_se = 273,
    set_sitename_param = 274,
    delete_sitename = 275,
    voice = 276,
    voice_is_play = 277,
    set_effect_param_by_btl_ef_file = 278,
    get_speed_rate_by_btl_ef_file = 279,
    set_shopdata = 280,
    set_shopitemdata = 281,
    boot_shop = 282,
    voice_pack_change = 283,
    set_menu_se_event_sound = 284,
    set_encount_arg = 285,
    get_encount_result = 286,
    btl_init_effect_param = 290,
    set_mapdef = 291,
    set_mapdef_shifttime = 292,
    btl_set_param = 293,
    btl_get_param = 294,
    set_reflection_offset = 297,
    init_se_menu_event = 298,
    term_se_menu_event = 299,
    se_menu_event_assign_id = 300,
    se_menu_event_entry = 301,
    move_pos_ex = 302,
    set_balloon_battle = 303,
    quake = 304,
    set_move_point = 305,
    set_move_chase = 306,
    set_move_escape = 307,
    set_chase = 308,
    set_escape = 309,
    set_voice_ai = 310,
    strncpy = 311,
    strncmp = 312,
    strncat = 313,
    stricmp = 314,
    strnicmp = 315,
    set_town = 316,
    set_area_weather = 317,
    vibration = 319,
    btl_check_auto_magic_arts = 320,
    btl_start_system_fix = 323,
    btl_end_system_fix = 324,
    btl_get_magic_arts_use_count = 325,
    btl_set_magic_arts_use_count = 326,
    set_scope_lookat_arc = 328,
    replace_mapdef = 329,
    reset_searchtre = 330,
    btl_disp_magic_arts_name = 331,
    btl_disp_magic_arts_string = 332,
    set_instance_person = 333,
    get_effect_scene_time = 334,
    btl_end_degenerate_bg = 335,
    set_label_ai = 336,
    chat = 337,
    get_chat_flag = 338,
    set_chat_flag = 339,
    set_savepoint = 340,
    delete_savepoint = 341,
    mtx_effect_player = 342,
    mtx_blend = 343,
    btl_start_fix_camera = 344,
    btl_end_fix_camera = 345,
    btl_set_fix_target = 346,
    btl_set_fix_param = 347,
    btl_set_fix_pos = 348,
    btl_set_fix_rot_y = 349,
    btl_set_fix_lock_camera = 350,
    btl_check_fix_reverse = 351,
    btl_init_camera_position = 352,
    btl_set_all_effect_pause = 355,
    btl_set_all_effect_fade = 356,
    btl_destroy_all_effect = 357,
    atan2f = 360,
    set_color_by_btl_color_pattern = 361,
    btl_before_effect_degenerate = 362,
    btl_set_effect_param = 364,
    btl_set_effect_param_f = 365,
    btl_get_effect_base_dir = 366,
    register_detach_work = 368,
    unregister_detach_work = 369,
    btl_set_effect_link = 370,
    get_chat_title = 371,
    move_pos_spline = 372,
    ef_set_btl_effect_param = 373,
    ef_set_btl_effect_param_f = 374,
    btl_cross_fade = 375,
    get_effect_bone_matrix = 376,
    vec_normalize = 377,
    vec_calc_angle = 378,
    world_to_screen = 379,
    set_scope_nameinfo = 380,
    set_gald = 381,
    add_gald = 382,
    get_gald = 383,
    set_pc_param = 384,
    get_pc_param = 385,
    get_party_top = 386,
    learn_magic_arts = 387,
    learn_enable_all_magic_arts = 388,
    learn_all_magic_arts = 389,
    register_move_on_npc = 390,
    unregister_move_on_npc = 391,
    delete_scene_charapack = 392,
    learn_all_skill = 393,
    set_wall_box = 394,
    is_party = 395,
    get_sorcererring_pos = 396,
    get_y = 397,
    allget_item = 398,
    movie = 399,
    snd_init_start = 400,
    snd_init_finish = 401,
    snd_event_start = 402,
    snd_event_finish = 403,
    se_lp_2d = 404,
    se_lp_point = 405,
    se_lp_cone = 406,
    se_lp_dir = 407,
    se_stop = 408,
    item_getcategory = 409,
    minigame = 410,
    get_magic_arts_use_count = 411,
    set_magic_arts_use_count = 412,
    register_ai_ex = 414,
    is_ai = 415,
    is_enable_all_ai = 416,
    set_move_stay = 417,
    set_chase_normal = 418,
    set_move_vertex = 419,
    set_effect_param_by_effect_parts_param = 421,
    set_parts_param = 422,
    set_parts_param_f = 423,
    get_parts_param = 424,
    get_parts_param_f = 425,
    btl_is_generate_effect = 426,
    init_effect_parts_common = 427,
    init_effect_parts_polygon = 428,
    ef_destroy_link_effect = 429,
    register_encount_resume = 431,
    get_recipe = 432,
    get_all_recipe = 433,
    se_os_2d = 434,
    se_os_point = 435,
    se_os_cone = 436,
    se_os_dir = 437,
    delete_person_range = 438,
    reset_scope = 439,
    reset_scope_circle = 440,
    reset_scope_linebox = 441,
    get_recipe_name = 442,
    btl_add_effect_generate_command = 443,
    btl_add_effect_group_command = 445,
    btl_boot_effect_group = 446,
    learn_all_fame = 447,
    learn_fame = 448,
    get_fame_name = 449,
    btl_add_effect_group_boot_command = 450,
    btl_regist_effect_group_effect = 451,
    set_party_top = 452,
    number_window = 453,
    get_result_number_window = 454,
    register_chara_debug_tag = 455,
    unregister_chara_debug_tag = 456,
    se_lp_line = 457,
    snd_sp_event = 458,
    repeat_value = 459,
    set_window_voice_interval = 460,
    btl_set_effect_generate_command = 464,
    ef_play_se = 465,
    ef_stop_se = 466,
    ef_is_play_se = 467,
    boot_menu = 468,
    boot_load = 469,
    boot_save = 470,
    clamp_value = 471,
    cancel_move_pos = 472,
    set_encount_music = 473,
    set_encount_music_default = 474,
    unregister_drag_object = 475,
    set_motion_alias = 477,
    reverb_off = 478,
    reverb_set = 479,
    set_situation = 480,
    set_test_play_point = 481,
    SetSynopsisMain = 482,
    SetSynopsisSub = 483,
    get_mg2_param = 484,
    set_mg2_param = 485,
    set_fs_effect_custom = 486,
    get_window_free_id = 487,
    get_balloon_free_id = 488,
    set_chara_debug_tag_user_string = 489,
    vec_scale = 490,
    vec_subtract = 491,
    register_stencil_order_chara_camera = 492,
    register_stencil_order_chara_value = 493,
    unregister_stencil_order_chara = 494,
    register_stencil_order_effect_camera = 495,
    register_stencil_order_effect_value = 496,
    register_stencil_order_effect_group = 497,
    unregister_stencil_order_effect = 498,
    btl_get_effect_param = 499,
    btl_get_effect_param_f = 500,
    ef_get_btl_effect_param = 501,
    ef_get_btl_effect_param_f = 502,
    set_window_input_wait_frame = 503,
    start_screen_capture = 504,
    end_screen_capture = 505,
    mtx_transpose = 506,
    mtx_inverse = 507,
    btl_get_pad_button_on = 508,
    btl_get_pad_button_repeat = 509,
    btl_get_pad_button_press = 510,
    set_select = 511,
    set_select_page_and_no = 512,
    info_window_ex = 513,
    get_lang = 514,
    quake3d = 515,
    vec_lerp = 516,
    vec_hermite = 517,
    get_window_param = 518,
    set_window_param = 519,
    btl_ground_break = 520,
    btl_get_pad_button_on_invalid_demo = 521,
    btl_reset_bone_hide = 523,
    btl_set_bone_hide = 524,
    set_ground_clip_stencil_order_effect = 525,
    get_chat_info = 526,
    set_battlebook_flag = 527,
    set_battlebook_flag_all = 528,
    set_comp_level = 529,
    set_all_synopsis = 530,
    btl_set_mystic_ground_enable = 531,
    btl_start_mystic_bg = 532,
    btl_end_mystic_bg = 533,
    btl_add_effect_attack_status_set = 534,
    get_part_of_directional_light = 535,
    set_part_of_directional_light = 536,
    btl_is_ground_break = 538,
    btl_get_ground_break_scene_name = 539,
    set_rotate_bone = 540,
    btl_is_learn_skill = 541,
    btl_is_use_skill = 542,
    btl_init_chain_history = 543,
    btl_set_work = 544,
    btl_get_work = 545,
    btl_set_work_f = 546,
    btl_get_work_f = 547,
    get_result_number_window_input = 548,
    set_rotate_bone_direct = 549,
    clear_rotate_bone = 550,
    btl_set_effect_at_blow_chara = 551,
    btl_exec_fb = 552,
    get_effect_2dp_string_param = 553,
    get_effect_2dp_string_length = 554,
    btl_get_bg_radius = 555,
    get_fame_point = 556,
    get_num_of_party = 557,
    set_order = 558,
    get_order = 559,
    set_fieldcontinent_no = 560,
    get_camera_axis = 562,
    set_mys_map = 563,
    btl_get_battle_state = 564,
    reset_lookat = 565,
    set_weather_ex = 566,
    boot_scene_name = 567,
    get_scene_name = 568,
    set_screen_blur_enable = 569,
    end_screen_blur = 570,
    set_screen_blur_level = 571,
    set_screen_blur_scale = 572,
    set_screen_blur_rotate = 573,
    set_screen_blur_move = 574,
    set_screen_blur_color = 575,
    get_v2v_axis = 576,
    get_camera_pos = 577,
    btl_start_event = 578,
    btl_end_event = 579,
    btl_create_chara_request = 580,
    btl_get_battle_time = 581,
    btl_exec_rover_item = 582,
    boot_grade_shop = 583,
    set_enm_book = 584,
    set_all_enm_book = 585,
    set_enm_encount = 586,
    set_all_enm_encount = 587,
    set_world_map = 588,
    set_all_world_map = 589,
    delete_ladder = 590,
    get_window_env_param = 591,
    set_window_env_param = 592,
    set_btl_rank_enable = 593,
    get_btl_rank_enable = 594,
    reset_footsteps_se_vol = 595,
    set_footsteps_se_vol = 596,
    push_order = 597,
    pop_order = 598,
    set_party_data = 599,
    add_party_data = 600,
    get_party_data = 601,
    set_sate_not_open_menu = 602,
    btl_set_camera_air_mode = 603,
    replace_chara_texture = 604,
    btl_set_cursor_draw = 605,
    btl_get_cursor_draw = 606,
    set_field_ship_position = 607,
    get_world_map = 608,
    btl_check_effect_magic_arts = 609,
    btl_check_generate_enemy = 610,
    btl_delete_chara_request = 611,
    btl_set_base_elem_def = 612,
    btl_get_base_elem_def = 613,
    btl_get_tactics = 614,
    get_btl_rank = 615,
    btl_search_area = 616,
    btl_get_priority_player_no = 617,
    btl_get_over_limits_point = 618,
    btl_get_over_limits_point_rate = 619,
    btl_get_over_limits_max_level = 620,
    btl_check_status_tc = 621,
    btl_shutdown = 622,
    btl_create_effect_group = 623,
    btl_set_battle_mode = 624,
    btl_get_battle_mode = 625,
    get_bone_pos = 626,
    get_bone_matrix = 627,
    get_item_book_rate = 628,
    get_enm_book_rate = 629,
    get_world_map_rate = 630,
    btl_set_effect_group_param = 631,
    btl_set_effect_group_param_f = 632,
    set_fs_se_fix_chara = 633,
    delete_footmark = 634,
    set_fame = 635,
    get_tre_param = 636,
    get_map_progress = 637,
    load_sebank = 638,
    delete_sebank = 639,
    set_vsigauge = 640,
    btl_register_stencil_order_ref_chara = 641,
    boot_load_ex_new = 642,
    check_load = 643,
    set_control_mode = 644,
    get_control_mode = 645,
    init_party_data = 646,
    reset_screen_blur_param = 647,
    get_grade_bit_flag = 648,
    get_chat_env = 649,
    set_chat_env = 650,
    btl_set_ui_enable = 651,
    btl_check_boss_battle = 652,
    btl_using_effect = 653,
    get_dead_last_pc = 654,
    set_fs_se_ex = 655,
    btl_sleep_reverb = 656,
    btl_wakeup_reverb = 657,
    add_window_string = 658,
    strrep = 659,
    btl_set_mission_complete = 660,
    btl_get_mission_complete = 661,
    change_window_page = 662,
    get_magic_arts_name = 663,
    get_skill_name = 664,
    check_dead_ll_enemy = 665,
    select_window_ex = 666,
    change_window_string = 667,
    item_window = 668,
    set_shop_tutorial = 669,
    btl_set_battle_bgm_volume = 670,
    set_dead_ll_enemy = 671,
    reset_window = 672,
    reset_debug_tag = 673,
    intersect = 674,
    set_skill = 676,
    btl_display_great = 677,
    btl_is_display_great = 678,
    btl_disp_auto_item_request = 679,
    get_stream_length = 680,
    ef_end_btl_effect_degenerate = 681,
    get_effect_calc = 682,
    btl_check_not_item_use = 683,
    btl_is_attack_magic = 684,
    btl_set_skill_param = 685,
    btl_set_skill_param_f = 686,
    btl_get_skill_param = 687,
    btl_get_skill_param_f = 688,
    btl_set_mystic_suspend_act = 689,
    btl_get_mystic_suspend_act = 690,
    get_treinfo = 691,
    btl_set_camera_pause = 692,
    set_ladder_se = 693,
    set_door_se = 694,
    get_maxitemcount = 695,
    btl_start_system_degenerate_bg = 696,
    btl_end_system_degenerate_bg = 697,
    btl_start_system_degenerate_chara = 698,
    btl_end_system_degenerate_chara = 699,
    btl_cut_change = 700,
    get_balloon_param = 701,
    set_balloon_param = 702,
    enum_window_free_id = 703,
    enum_balloon_free_id = 704,
    btl_set_ctrl_color_bg = 705,
    btl_set_ctrl_color_chara = 706,
    btl_order_effect_event = 707,
    btl_quake = 708,
    btl_start_act_quake = 709,
    btl_end_act_quake = 710,
    btl_set_act_quake = 711,
    btl_set_result_process_stop = 712,
    btl_get_result_process_stop = 713,
    btl_rewrite_enemy_group = 714,
    btl_set_enable_limit_time = 715,
    btl_set_limit_time = 716,
    btl_get_limit_time = 717,
    btl_check_create_chara_cost = 718,
    btl_load_request_chara_pack = 719,
    btl_is_load_chara_pack = 720,
    btl_register_chara_pack = 721,
    btl_unregister_chara_pack = 722,
    number_window_ex = 723,
    get_now_result_number_window = 724,
    se_lp_vol = 725,
    btl_set_bg_layer_speed = 726,
    btl_get_bg_layer_speed = 727,
    btl_get_mortuary_survival_count = 728,
    btl_clear_status_tc = 729,
    btl_get_mystic_chara_no = 730,
    get_z_stencil_order_effect = 731,
    set_status_trouble = 732,
    register_stencil_order_effect_group2 = 733,
    set_savepoint_index = 734,
    set_clear_flag = 735,
    get_clear_flag = 736,
    get_load_data_clear_flag = 737,
    btl_printf = 738,
    craft_window = 739,
    destroy_thread = 740,
    is_thread = 741,
    btl_destroy_all_effect_mystic = 742,
    add_window_string_by_id = 743,
    add_window_string_ex = 744,
    btl_get_magic_arts_tp_cost = 745,
    delete_door = 746,
    change_act_pos = 747,
    btl_create_actinfo = 748,
    btl_delete_actinfo = 749,
    is_learn_fame = 750,
    get_itemtext = 751,
    get_cook_level = 752,
    set_cook_level_max_all = 753,
    btl_set_enable_dead_count = 754,
    btl_set_dead_count_target = 755,
    btl_set_change_control_mode = 756,
    btl_load_sebank = 757,
    btl_is_load_sebank = 758,
    btl_delete_sebank = 759,
    enum_dl_chat = 760,
    get_chat_flag_by_unique = 761,
    set_chat_flag_by_unique = 762,
    strstr = 763,
    btl_fade_out = 764,
    btl_fade_in = 765,
    btl_get_magic_effect_count = 766,
    btl_get_magic_effect_param = 767,
    btl_get_magic_effect_param_f = 768,
    btl_is_fix_camera = 769,
    is_learn_recipe = 770,
    get_recipe_max = 771,
    get_recipe_id = 772,
    allclear_item = 773,
    push_itemcount = 774,
    pop_itemcount = 775,
    get_env_ai = 776,
    set_env_ai = 777,
    btl_set_encount_camera_enable = 778,
    set_forbit_special_flag = 779,
    set_cook_level = 780,
    set_recipe = 781,
    btl_set_tutorial_run = 782,
    btl_check_boost_magic_arts = 783,
    set_is_cook = 784,
    set_is_camp = 785,
    delete_barrier = 786,
    set_grade_bit_flag = 787,
    btl_hide_auto_item_request = 788,
    get_comp_level = 789,
    delete_scenario_charapack = 790,
    set_result_voice_bit_flag = 791,
    get_result_voice_bit_flag = 792,
    btl_is_learn_magic_arts = 793,
    btl_get_range_party_num = 794,
    btl_get_range_enemy_num = 795,
    btl_get_range_chara_num = 796,
    btl_get_angle_party_num = 797,
    btl_get_angle_enemy_num = 798,
    btl_get_angle_chara_num = 799,
    btl_get_angle_range_party_num = 800,
    btl_get_angle_range_enemy_num = 801,
    btl_get_angle_range_chara_num = 802,
    _btl_ai_check_guard = 803,
    _btl_ai_check_guard_magic = 804,
    _btl_ai_get_magic_arts_id = 805,
    _btl_ai_get_request_ovl_level = 806,
    set_magic_arts = 807,
    get_magic_arts = 808,
    is_change_party_top = 809,
    btl_learn_magic_arts = 810,
    btl_clear_effect_rbm = 811,
    btl_clear_effect_ebm = 812,
    btl_add_effect_hit_link = 813,
    btl_clear_light_sword_range = 814,
    btl_add_light_sword_range = 815,
    create_npc = 816,
    delete_npc = 817,
    get_npc_env = 818,
    set_npc_env = 819,
    get_npc_param = 820,
    set_npc_param = 821,
    vec_copy = 822,
    vec_set = 823,
    btl_register_stencil_order_ref_effect = 824,
    is_magic_arts_id = 825,
    is_use_magic_arts = 826,
    forget_magic_arts = 827,
    forget_all_magic_arts = 828,
    get_magic_arts_name_no_rubi = 829,
    order_npc_command = 830,
    get_magic_arts_data_num = 831,
    get_magic_arts_id_from_index = 832,
    get_npc_string = 833,
    set_npc_string = 834,
    get_item_by_category = 835,
    get_item_by_category2 = 836,
    is_map_object = 837,
    is_npc = 838,
    set_motion_time = 839,
    get_motion_time = 840,
    btl_get_xtm_area_num = 841,
    btl_get_xtm_area_map_num = 842,
    btl_get_xtm_area_map_name = 843,
    btl_get_xtm_map_width = 844,
    btl_get_xtm_map_height = 845,
    btl_get_xtm_map_param = 846,
    btl_get_xtm_map_param_f = 847,
    set_symbol_encount_param = 848,
    get_symbol_encount_param = 849,
    btl_change_map = 850,
    btl_is_change_map = 851,
    boot_config = 852,
    get_treasure_param = 853,
    set_treasure_param = 854,
    btl_get_random_enemy_group_id = 855,
    btl_set_mortuary_force_chara = 856,
    btl_get_mortuary_force_chara = 857,
    btl_create_enemy_group_request = 858,
    btl_check_create_remainder_cost = 859,
    btl_get_create_remainder_count = 860,
    btl_set_effect_group_boot_command = 861,
    btl_exec_steal_gald = 862,
    btl_set_magic_arts_string_value_i = 863,
    set_treasure_box = 864,
    set_treasure_box_by_id = 865,
    set_treasure_box_by_data = 866,
    get_treasure_env = 867,
    set_treasure_env = 868,
    set_search_box = 869,
    set_search_box_by_id = 870,
    get_treasure_flag = 871,
    set_treasure_flag = 872,
    btl_get_formation_pc = 873,
    get_treasure_file_data = 874,
    execute_treasure_box = 875,
    execute_treasure_box_by_data = 876,
    btl_get_xtm_tre_param = 877,
    btl_get_xtm_tre_param_f = 878,
    btl_set_xtm_warp_value = 879,
    btl_get_xtm_warp_value = 880,
    btl_get_xtm_rank = 881,
    btl_get_xtm_map_name = 882,
    enum_npc_chara_no = 883,
    get_npc_file_data = 884,
    get_npc_file_data_by_id = 885,
    btl_set_scope = 886,
    btl_delete_scope = 887,
    _btl_ai_set_auto_item_request = 888,
    create_npc_by_id_ex = 889,
    get_npc_field_area_data = 890,
    get_ground_angle = 891,
    btl_destroy_effect_group = 892,
    btl_start_extra_camera = 893,
    btl_end_extra_camera = 894,
    btl_set_fix_param_ex = 895,
    btl_is_move_extra_camera = 896,
    set_pos_ex = 897,
    btl_set_player_lr = 898,
    get_pad_stick = 899,
    get_ground_normal = 900,
    register_npc = 901,
    get_npc_file_data_by_label = 902,
    unregister_npc = 903,
    rand_range = 904,
    is_motion = 905,
    btl_set_camera_off_mode = 906,
    btl_reset_not_item_use = 907,
    btl_reset_escape = 908,
    start_item_infomation = 909,
    get_effect_2dp_string_param_ex = 910,
    get_effect_2dp_string_length_ex = 911,
    boot_sound_theater = 912,
    btl_exec_poker = 913,
    set_camera_pause = 914,
    get_gadget_arg_ex = 915,
    btl_set_effect_order_mystic = 916,
    get_target_platform = 917,
    set_minigame_pause = 918,
    btl_get_card_data_from_id = 919,
    btl_get_card_force_from_id = 920,
    btl_get_card_id_from_list = 921,
    btl_alloc_chara_no = 922,
    btl_free_chara_no = 923,
    btl_set_not_item_use_time = 924,
    btl_set_not_item_use_time_max = 925,
    btl_flush = 926,
    btl_set_result_exp_rate = 927,
    btl_get_result_exp_rate = 928,
    btl_set_result_gald_rate = 929,
    btl_get_result_gald_rate = 930,
    btl_is_enable_use_magic_arts_list = 931,
    btl_disp_magic_arts_name_priority = 932,
    btl_disp_magic_arts_string_priority = 933,
    btl_disp_magic_arts_name_priority_frame = 934,
    btl_disp_magic_arts_string_priority_frame = 935,
    get_clear_tougi30_time = 936,
    btl_get_camera_rot = 937,
    get_string_count = 938,
    create_npc_by_label = 939,
    btl_set_mystic_bg_ground_clip = 940,
    get_motion_totaltime = 941,
    get_snowboard_buffer = 942,
    get_snowboard_buffer_size = 943,
    set_snowboard_buffer = 944,
    request_upload_leader_board = 945,
    get_item_request_setting = 946,
    btl_get_debug_param = 947,
    get_field_map_dog_develop_id = 948,
    set_field_map_dog_develop_id = 949,
    get_world_map_data = 950,
    set_world_map_data = 951,
    get_baction_floor = 952,
    set_baction_floor = 953,
    get_field_map_env = 954,
    get_item_count = 955,
    set_item_count = 956,
    add_item_count = 957,
    get_item_param = 958,
    get_item_string = 959,
    btl_call_debug_function = 960,
    vec_length_sq = 961,
    is_treasure = 962,
    enum_treasure_id = 963,
    get_treasure_position = 964,
    get_synopsis_id = 965,
    set_synopsis_id = 966,
    get_synopsis_param = 967,
    gen_uid_from_ptr = 968,
    is_initial_chunk_loaded = 969,
    delete_person_blocking = 970,
    get_audio_lang = 971,
    get_mouse_position = 972,
    is_mouse_activated = 973,
    set_logo_step = 974,
}

public enum ScfomSyscall_V31600 // Tales of Xillia
{
    print = 60, // Guessed
    set_motion = 112,
}