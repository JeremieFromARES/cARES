using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using static cARES.ARES_std;
using static cARES.Lib;
using static cARES.Program;

namespace cARES
{
    public static class Parser
    {
        /// <summary>
        /// Char by Char inspection : Removes comments and seperates string from other tokens
        /// </summary>
        public static void FirstPass()
        {
            Line lin = new();
            Token tok = new();

            bool is_string = false;
            bool discard = false;

            string cur_char = "";
            string trim = "(\\s)*$|^(\\s)*";

            long line_bounds = source_lines.Count();
            long char_bounds = 0;

            for (int l = 0; l < line_bounds; l++)
            {
                lin.tokens = new();
                lin.number = l + 1;
                tok.content = ""; tok.type = TokenType.UnParsed;
                is_string = false; discard = false;
                char_bounds = source_lines[l].Count();

                for (int c = 0; c < char_bounds; c++)
                {
                    cur_char = source_lines[l][c].ToString();
                    if (c > 1 && !discard)
                    {
                        if (cur_char == "\"")
                        {
                            if ((source_lines[l][c - 1].ToString() == "\\") == false || (source_lines[l][c - 1].ToString() == "\\" && source_lines[l][c - 2].ToString() == "\\") == true)
                            {
                                is_string = !is_string;
                                if (!is_string)
                                {
                                    tok.type = TokenType.String;
                                }
                                else
                                {
                                    tok.content = Regex.Replace(tok.content, trim, "");
                                    tok.type = TokenType.UnParsed;
                                }
                                lin.tokens.Add(tok);
                                tok.content = "";
                                continue;
                            }
                        }
                    }
                    if (c < char_bounds && !is_string)
                    {
                        if ((cur_char == "\\" && source_lines[l][c + 1].ToString() == "\\") || (cur_char == "/" && source_lines[l][c + 1].ToString() == "/"))
                        {
                            discard = true;
                            tok.content = Regex.Replace(tok.content, trim, "");
                            tok.type = TokenType.UnParsed;
                            lin.tokens.Add(tok);
                            tok.content = "";
                        }
                    }
                    if (!discard) { tok.content += cur_char; }
                }
                if (!is_string) { tok.type = TokenType.UnParsed; tok.content = Regex.Replace(tok.content, trim, ""); }
                lin.tokens.Add(tok);
                parsed_lines_pass1.Add(lin);
            }

            source_lines.Clear();
        }
        /// <summary>
        /// Tokenization : Gives an identifiable type to tokens
        /// </summary>
        public static void SecondPass()
        {
            Line temp_line = new();
            Token temp_token = new();
            Token end_of_line = new();

            string temp_string = "";

            List<string> temp_array;

            int line_counter = 0;
            int depth = 0;

            foreach (Line l in parsed_lines_pass1) {
                line_counter += 1;
                temp_line.tokens.Clear();

                foreach (Token current_token in l.tokens) {
                    if (current_token.type != TokenType.String)
                    {

                        // Split tokens
                        temp_string = current_token.content;
                        foreach (string kw in ARES_std.keywords.Values) { temp_string = temp_string.Replace(kw, " " + kw + " "); }
                        foreach (string op in ARES_std.operators.Values) { temp_string = temp_string.Replace(op, " " + op + " "); }
                        temp_array = temp_string.Split(" ").ToList();

                        foreach (string stoken in temp_array) {

                            temp_token.content = stoken;
                            if (ARES_std.keywords.ContainsValue(stoken))
                            {
                                if (Regex.IsMatch(stoken, "\\("))
                                {
                                    temp_token.type = TokenType.ArgStart;
                                    temp_token.content = "";
                                    depth += 1;
                                }
                                else if (Regex.IsMatch(stoken, "\\)"))
                                {
                                    temp_token.type = TokenType.ArgEnd;
                                    temp_token.content = "";
                                    depth -= 1;
                                }
                                else if (Regex.IsMatch(stoken, "\\<"))
                                {
                                    temp_token.type = TokenType.TypeDefStart;
                                    temp_token.content = "";
                                }
                                else if (Regex.IsMatch(stoken, "\\>"))
                                {
                                    temp_token.type = TokenType.TypeDefEnd;
                                    temp_token.content = "";
                                }
                                else if (Regex.IsMatch(stoken, "\\{"))
                                {
                                    temp_token.type = TokenType.BrackStart;
                                    temp_token.content = "";
                                    depth += 1;
                                }
                                else if (Regex.IsMatch(stoken, "\\}"))
                                {
                                    temp_token.type = TokenType.BrackEnd;
                                    temp_token.content = "";
                                    depth -= 1;
                                }
                                else if (Regex.IsMatch(stoken, "\\,"))
                                {
                                    temp_token.type = TokenType.ArgDelim;
                                    temp_token.content = "";
                                }
                                else if (Regex.IsMatch(stoken, "\\;"))
                                {
                                    temp_token.type = TokenType.LineDelim;
                                    temp_token.content = "";
                                }
                                else
                                {
                                    temp_token.type = TokenType.KeyWord;
                                }
                            }
                            else if (ARES_std.operators.ContainsValue(stoken))
                            {
                                if (Regex.IsMatch(stoken, "\\."))
                                {
                                    temp_token.type = TokenType.Dot;
                                    temp_token.content = "";
                                }
                                else
                                {
                                    temp_token.type = TokenType.Operator;
                                }
                            }
                            else if (ARES_std.types.ContainsKey(stoken))
                            {
                                temp_token.type = TokenType.Type;
                            }
                            else if (Regex.IsMatch(stoken, "(\\d+\\.\\d+)|\\d+"))
                            {
                                temp_token.type = TokenType.Number;
                            }
                            else if (!Regex.IsMatch(stoken, "\\W|((\\d)\\w+)"))
                            {
                                temp_token.type = TokenType.Named;
                            }
                            else
                            {
                                temp_token.type = TokenType.UnDetermined;
                            }
                            temp_line.tokens.Add(temp_token);
                        }
                    }
                    else
                    {
                        temp_line.tokens.Add(current_token);
                    }
                }
                end_of_line.content = "";
                end_of_line.type = TokenType.EndOfLine;
                temp_line.tokens.Add(end_of_line);
                temp_line.number = line_counter;
                parsed_lines_pass2.Add(temp_line);
            }

            parsed_lines_pass1.Clear();
        }
        /// <summary>
        /// Data Collection : Fetches as much data as possible before turning tokens into instructions
        /// </summary>
        public static void ThirdPass()
        {
            Token temp_token;
            //ScopeNode temp_scope;
            ScopeNode globalScope = new ScopeNode()
            {
                label = "__ARES_GLOBAL_SCOPE__",
                type = ScopeType.Space
            };
            ScopeNode current_scope = globalScope;

            foreach (Line l in parsed_lines_pass2)
            {
                for (int i = 0; i < l.tokens.Count; i++)
                {
                    temp_token = l.tokens[i];
        //Variables
                    if (temp_token.type == TokenType.Type && l.tokens[i + 1].type == TokenType.Named)
                    {
                        if (l.tokens[i + 1].type == TokenType.Named)
                        {
                            ScopeNode temp_node = new ScopeNode()
                            {
                                datatype = temp_token.content,
                                label = l.tokens[i + 1].content,
                                type = ScopeType.Variable,
                                parent = current_scope
                            };
                            current_scope.children.Add(temp_node.label, temp_node);

                //Assignment
                            if (l.tokens[i + 2].type == TokenType.Operator && l.tokens[i + 2].content == "=")
                            {
                                if (l.tokens[i + 3].type == TokenType.EndOfLine)
                                {
                                    Raise(l.number, "expected value after operator \"=\".");
                                }
                            }
                        }
                        else
                        {
                            Raise(l.number, "expected name after type \"" + temp_token.content + "\".");
                        }
                    }
        //Code block
                    else if ((i == 0 || l.tokens[i - 1].type == TokenType.LineDelim) && temp_token.type == TokenType.KeyWord && Regex.IsMatch(temp_token.content, "fnc|function|obj|object|spc|space"))
                    {
                        if (l.tokens[i + 1].type == TokenType.Named)
                        {
                //Function
                            if (Regex.IsMatch(temp_token.content,"fnc|function"))
                            {
                                ScopeNode temp_node = new ScopeNode()
                                {
                                    datatype = temp_token.content,
                                    label = l.tokens[i + 1].content,
                                    type = ScopeType.Function,
                                    parent = current_scope
                                };
                                current_scope.children.Add(temp_node.label, temp_node);
                                current_scope = temp_node;
                            }
                //Object
                            else if (Regex.IsMatch(temp_token.content, "obj|object"))
                            {
                                ScopeNode temp_node = new ScopeNode()
                                {
                                    datatype = temp_token.content,
                                    label = l.tokens[i + 1].content,
                                    type = ScopeType.Object,
                                    parent = current_scope
                                };
                                current_scope.children.Add(temp_node.label, temp_node);
                                current_scope = temp_node;
                            }
                //Space
                            else if (Regex.IsMatch(temp_token.content, "spc|space"))
                            {
                                ScopeNode temp_node = new ScopeNode()
                                {
                                    datatype = temp_token.content,
                                    label = l.tokens[i + 1].content,
                                    type = ScopeType.Space,
                                    parent = current_scope
                                };
                                current_scope.children.Add(temp_node.label, temp_node);
                                current_scope = temp_node;
                            }
                        }
                        else
                        {
                            Raise(l.number, "expected name after keyword \"" + temp_token.content + "\".");
                        }
                    }
        //End code block
                    else if (temp_token.type == TokenType.BrackEnd || (temp_token.type == TokenType.Dot && l.tokens[i + 1].type == TokenType.Dot))
                    {
                        if (temp_token.type == TokenType.BrackEnd || (l.tokens[i + 2].type == TokenType.KeyWord && Regex.IsMatch(l.tokens[i + 2].content, "fnc|function|spc|space|obj|object|if|loop")))
                        {

                        }
                        else
                        {
                            Raise(l.number, "expected keyword after \"..\".");
                        }
                    }
                }
            }
        }
    }
}
