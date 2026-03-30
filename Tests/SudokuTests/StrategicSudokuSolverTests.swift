//
//  StrategicSudokuSolverTests.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing
import Utilities

@testable
import Sudoku

@Test
fileprivate func testStrategicSudokuSolver_initializer() {
    // Arrange
    let array = randomArray()
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { array[$0 * 9 + $1] }
    
    // Act
    let result = StrategicSudokuSolver(sudoku)
    
    // Assert
    #expect(result.moves.isEmpty)
    expectEqual(sudoku, result.sudoku)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_notSolvable() {
    // Arrange
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { _, _ in 0 }
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    for strategy in SudokuSolverStrategy.allCases {
        // Act
        let result = solver.solve(for: strategy, using: &generator)
        
        // Assert
        #expect(result == nil)
        #expect(solver.moves.isEmpty)
        expectEmpty(solver.sudoku)
    }
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_block() {
    // Arrange
    let string = "800739006370465000040182009000600040054300610060500000400853070000271064100940002"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 5)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 8)
    #expect(result!.strategy == .fullHouse)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_column() {
    // Arrange
    let string = "200060000083090000700821900006073000090682040000450100008935004000000290000040008"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 1)
    #expect(location.column == 4)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 7)
    #expect(result!.strategy == .fullHouse)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_fullHouse_row() {
    // Arrange
    let string = "207000000080090000030600800008164900692785304001320500009001020000040090000000408"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .fullHouse, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 1)
    #expect(location.column == 7)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 4)
    #expect(result!.strategy == .fullHouse)
}

@Test
fileprivate func testStrategicSudokuSolver_solve_solvable_nakedSingle() {
    // Arrange
    let string = "412736589000000106568010370000850210100000008087090000030070865800000000000908401"
    var sudoku = MockSudoku()
    
    sudoku.subscriptClosure = {
        row, column in
        let index = string.index(string.startIndex, offsetBy: row * 9 + column)
        return string[index].wholeNumberValue!
    }
    
    var solver = StrategicSudokuSolver(sudoku)
    var generator = MockRandomNumberGenerator()
    generator.nextClosure = { 1 }
    
    // Act
    let result = solver.solve(for: .nakedSingle, using: &generator)
    
    // Assert
    let location = result!.locations.single!
    #expect(location.addedValue == 6)
    #expect(location.column == 6)
    #expect(location.removedCandidates.isEmpty)
    #expect(location.row == 5)
    #expect(result!.strategy == .nakedSingle)
}
