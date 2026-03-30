//
//  StrategicSudokuSolverTests.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing

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
