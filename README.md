# SharpTxt

A Simple CLI Text-Editor, written in C# .NET 8.

Project done as part of C# Learning Process.

## Table of Contents

- [SharpTxt](#sharptxt)
  - [Table of Contents](#table-of-contents)
  - [Dependencies](#dependencies)
  - [Setup the Project](#setup-the-project)
  - [Running the Project](#running-the-project)

## Dependencies

- .NET 8 Runtime

## Setup the Project

- Clone the repository:
  ```bash
  git clone https://github.com/mukund-ks/SharpTxt.git
  ```

- Move into cloned repository:
  ```bash
  cd SharpTxt
  ```

- Compile the project:
  ```bash
  dotnet build -c Release -o ./release
  ```

## Running the Project

- Move into the `release` directory:
  ```bash
  cd release
  ```

- Run SharpTxt:
  ```bash
  ./SharpTxt <path-to-txt-file>
  ```
  or
  ```bash
  ./SharpTxt
  ```
  On prompt, enter the path to an existing txt file. Leaving the prompt empty will create a new txt file named `untitled.txt`.
