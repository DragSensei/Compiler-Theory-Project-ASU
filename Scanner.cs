using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Int, Float, String, Read, Write, Repeat, Until, If, ElseIf,
    Else, Then, Return, Endl, Main, 

    Semicolon, Comma, LParanthesis, RParanthesis, LCurlyBracket, RCurlyBracket,
    
    AssignmentOp,
    EqualOp,
    LessThanOp,
    GreaterThanOp,
    NotEqualOp,
    ANDOp,
    OROp,
    
    PlusOp, MinusOp, MultiplyOp, DivideOp, 
    
    Identifier, 
    Constant,
    StringLit,
    Comment
}
namespace JASON_Compiler
{
    public class Token
    {
       public string lex;
       public Token_Class token_type;
        public Token()
        {
        }
           
        public Token(string lex, Token_Class token_type)
        {
            this.lex = lex;
            this.token_type = token_type;
        }
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("INT", Token_Class.Int);
            ReservedWords.Add("FLOAT", Token_Class.Float);
            ReservedWords.Add("STRING", Token_Class.String);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("WRITE", Token_Class.Write);
            ReservedWords.Add("REPEAT", Token_Class.Repeat);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("ELSEIF", Token_Class.ElseIf);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("RETURN", Token_Class.Return);
            ReservedWords.Add("ENDL", Token_Class.Endl);
            ReservedWords.Add("MAIN", Token_Class.Main);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyBracket);
            Operators.Add("}", Token_Class.RCurlyBracket);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.ANDOp);
            Operators.Add("||", Token_Class.OROp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i + 1;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                Token Tok = new Token();
                Tok.lex = CurrentLexeme;

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (char.IsLetter(CurrentChar)) 
                {
                    while (j < SourceCode.Length && char.IsLetterOrDigit(SourceCode[j]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if(char.IsDigit(CurrentChar))
                {
                    while (j < SourceCode.Length && char.IsDigit(SourceCode[j]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length && SourceCode[j] == '.' && j + 1 < SourceCode.Length && char.IsDigit(SourceCode[j + 1]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        while (j < SourceCode.Length && char.IsDigit(SourceCode[j]))
                        { 
                            CurrentLexeme += SourceCode[j]; 
                            j++;
                        }
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if (CurrentChar == '"')
                {
                    j = i + 1;
                    while (j < SourceCode.Length && SourceCode[j] != '"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length && SourceCode[j] == '"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        Tok.lex = CurrentLexeme;
                        Tok.token_type = Token_Class.StringLit;
                        Tokens.Add(Tok);
                    }
                    else
                    {
                        Errors.Error_List.Add("Unterminated string literal");
                    }
                    i = j - 1;
                    continue;
                }
                else if(CurrentChar == '/' && j < SourceCode.Length && SourceCode[j] == '*')
                {
                    CurrentLexeme += SourceCode[j];
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '*' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '/')
                        {
                            CurrentLexeme += SourceCode[j];
                            CurrentLexeme += SourceCode[j + 1];
                            j += 2;
                            break; 
                        }
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    if (!CurrentLexeme.EndsWith("*/"))
                    {
                        Errors.Error_List.Add("Unterminated comment");
                    }
                    
                    i = j - 1;
                    continue;
                }
                else
                {
                    if (j < SourceCode.Length)
                    {
                        string twoCharLexeme = CurrentLexeme + SourceCode[j].ToString();
                        if (Operators.ContainsKey(twoCharLexeme))
                        {
                            CurrentLexeme = twoCharLexeme;
                            j++;
                            i = j - 1;
                            FindTokenClass(CurrentLexeme);
                            continue;
                        }
                    }

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            if (ReservedWords.ContainsKey(Lex.ToUpper()))
            {
                Tok.token_type = ReservedWords[Lex.ToUpper()];
                Tokens.Add(Tok);
                return;
            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
                return;
            }            
            else if (Operators.ContainsKey((Lex)))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
                return;
            }
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
                return;
            }
            else
            {
                Errors.Error_List.Add($"Unknown token: {Lex}");
            }
        }
        
        bool isIdentifier(string lex)
        {
            bool isValid=true;

            if (string.IsNullOrEmpty(lex) || !char.IsLetter(lex[0]))
            {
                return false;
            }

            for (int i = 1; i < lex.Length; i++)
            {
                if (!char.IsLetterOrDigit(lex[i]))
                    return false;
            }
            
            return isValid;
        }
        
        bool isConstant(string lex)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(lex)) return false;

            int dotCount = 0;

            foreach (char c in lex)
            {
                if (c == '.')
                {
                    dotCount++;
                    if (dotCount > 1)
                        return false;
                }
                else if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return isValid;
        }
    }
}