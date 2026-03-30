//
//  RecursiveSudokuSolverTests.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing

@testable
import Sudoku

@Test
fileprivate func testRecursiveSudokuSolver_initializer() {
    // Arrange
    let array = randomArray()
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { array[$0 * 9 + $1] }
    
    // Act
    let result = RecursiveSudokuSolver(sudoku)
    
    // Assert
    #expect(result.moves.isEmpty)
    expectEqual(sudoku, result.sudoku)
}

@Test
fileprivate func testRecursiveSudokuSolver_solve_notSolved_notSolvable() {
    // Arrange
    let string = "123456789456789123789123456214365897365897214897214365531642978642978531978531620"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = RecursiveSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(using: &generator)
    
    // Assert
    #expect(!result)
    #expect(solver.moves.isEmpty)
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testRecursiveSudokuSolver_solve_notSolved_solvable() {
    // Arrange
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { _, _ in 0 }
    var solver = RecursiveSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(using: &generator)
    
    // Assert
    let string = "123456789456789123789123456214365897365897214897214365531642978642978531978531642"
    #expect(result)
    #expect(solver.moves.count == 81)
    var stringIndex = string.startIndex

    for row in 0..<9 {
        for column in 0..<9 {
            let move = solver.moves[row * 9 + column]
            #expect(move.locations.count == 1)
            let location = move.locations.first!
            #expect(location.addedValue == string[stringIndex].wholeNumberValue)
            #expect(location.column == column)
            #expect(location.removedCandidates.isEmpty)
            #expect(location.row == row)
            #expect(move.strategy == nil)
            stringIndex = string.index(after: stringIndex)
        }
    }
    
    expectEqual(string, solver.sudoku)
}

@Test
fileprivate func testRecursiveSudokuSolver_solve_solved() {
    // Arrange
    let string = "123456789456789123789123456214365897365897214897214365531642978642978531978531642"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = RecursiveSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(using: &generator)
    
    // Assert
    #expect(result)
    #expect(solver.moves.isEmpty)
    expectEqual(string, solver.sudoku)
}
