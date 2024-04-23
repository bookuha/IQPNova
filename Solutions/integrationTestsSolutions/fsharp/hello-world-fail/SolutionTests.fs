module SolutionTests

open Xunit
open FsUnit.Xunit
open Exercism.Tests
open Solution

[<Fact>]
let ``helloWorld returns Hello, world!`` () =
    let actual = helloWorld ()
    Assert.Equal("Hello, world!", actual)