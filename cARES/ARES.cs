using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cARES
{
    public static class ARES_std
    {
        public enum TokenType
        {
            UnParsed,       // 0
            String,         // 1
            Type,           // 2
            KeyWord,        // 3
            Named,          // 4
            Operator,       // 5
            Number,         // 6
            ArgStart,       // 7
            ArgEnd,         // 8
            ArgDelim,       // 9
            TypeDefStart,   // 10
            TypeDefEnd,     // 11
            BrackStart,     // 12
            BrackEnd,       // 13
            Dot,            // 14
            EndOfLine,      // 15
            LineDelim,      // 16
            UnDetermined,   // 17
            ExpPlaceholder, // 18
        }
        public struct Token
        {
            public Token() { }
            public string content = "";
            public TokenType type = TokenType.UnParsed;
        }
        public struct Line
        {
            public Line() { }
            public List<Token> tokens = new();
            public long number = 0;
        }
        public enum ScopeType
        {
            Undefined,
            Variable,
            Function,
            Object,
            Space,
            Other,
        }
        /// <summary>
        /// Node Tree representing the relationship of scopes in ARES source code.
        /// </summary>
        public abstract class ScopeTree
        {
            public ScopeTree() { }
            /// <summary>
            /// Holds the ScopeNodes that are defined in the global scope.
            /// <para>Key: label</para>
            /// <para>Value: ScopeNode</para>
            /// </summary>
            public Dictionary<string, ScopeNode> children = new();
        }
        /// <summary>
        /// Node representing a scope or object inside a scope, can hold other ScopeNode objects.
        /// </summary>
        public class ScopeNode : ScopeTree
        {
            public ScopeNode() { }
            /// <summary>
            /// Type of Node: Can be Variable, Function, Object etc...
            /// </summary>
            public ScopeNode?   parent = null;
            public ScopeType    type = ScopeType.Undefined;
            public string       label = "";
            public string       datatype = "";
            public bool         scope_bnd = false;
            /// <summary>
            /// <para>Key: label</para>
            /// <para>Value: datatype</para>
            /// </summary>
            public Dictionary<string, string> arguments = new();
        }
        public static Dictionary<string, string> keywords = new()
        {
            {"kw_args_delim",       ","},
            {"kw_line_delim",       ";"},
            {"kw_args_start",       "("},
            {"kw_args_end",         ")"},
            {"kw_typedef_start",    "<"},
            {"kw_typedef_end",      ">"},
            {"kw_brackets_start",   "{"},
            {"kw_brackets_end",     "}"},
            {"kw_sqbrackets_start", "["},
            {"kw_sqbrackets_end",   "]"},
            {"kw_block_end",        ".."},
            {"kw_function_start",   "fnc|function"},
            {"kw_space_start",      "spc|space"},
            {"kw_object_start",     "obj|object"},
            {"kw_bool_true",        "true"},
            {"kw_bool_false",       "false"},
            {"kw_if",               "if"},
            {"kw_else",             "else"},
            {"kw_from",             "from"}
        };

        public static Dictionary<string, string> operators = new()
        {
            {"op_plus",             "+"},
            {"op_minus",            "-"},
            {"op_divide",           "/"},
            {"op_mult",             "*"},
            {"op_dot",              "."},
            {"op_assign",           "="},
            {"op_modulo",           "%"},
            {"op_not",              "!"}
        };

        public static Dictionary<string, string> types = new()
        {
            {"str",                 "ARES::Types::String"},
            {"arr<#0>",             "ARES::Types::Array<#0>"},
            {"map<#0,#1>",          "ARES::Types::Map<#0,#1>"},
            {"file",                "ARES::Types::File"},
            {"lng",                 "long long int"},
            {"int",                 "long int"},
            {"sht",                 "int"},
            {"dci",                 "long double"},
            {"dbl",                 "double"},
            {"flt",                 "float"}
        };
    }
}
