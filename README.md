# CSLox
This is an implementation of a tree-walk interpreter, based on the one outline in part one of ["Crafting Interpreters"](https://craftinginterpreters.com/) by Bob Nystrom. It's implemented in C# (hence the name cslox) and features a number of improvements on the original code. Some are a result of the additional challenges section in the book, other my own architectural improvements (I didn't like some of the decisions).

## Improvements

### Challenges
Several of the challenges resulted in improvements to the interpreter and the language:

 - Block comments.
 - [Comma operator](https://en.wikipedia.org/wiki/Comma_operator).
 - Ternary operator.
 - Heterogenous string concatenation ("Apple" + 3 = "Apple3").
 - Errors when accessing uninitialized variables.
 - Break statement in loops
 - Reporting an error if a local variable is never used.

### Architecture

- *REPL*: Separated REPL and File evaluation in two different classes, that implement a common base class. 
- Error Handling: I found the original error handling a bit convoluted. In addition it coupled every element of scanning, parsing and interpretation to the entry Lox class. To give the elements of the interpreter more autonomy, I made every important method (Scan, Parse, Resolve) receive a list of errors as a parameter. From then on it's the responsibility of the caller (*REPL*, *FileEvaluator*, or other) to figure out what to do with those errors. 
- ASTGen: Moved AST descriptions to their own files and gave them ",astdef" filetype. I found that it creates a better separation between logic and data. 

### TODO

 - Lambdas.
 - Continue statement.
 - Variable lookup by index.
 - Static methods.
 - Getters & setters.
