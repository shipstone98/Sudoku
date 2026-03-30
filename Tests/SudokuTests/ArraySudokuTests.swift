//
//  ArraySudokuTests.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing

@testable
import Sudoku

@Test
fileprivate func testArraySudoku_subscript() {
    // Arrange
    let cells = randomArray()
    var sudoku = ArraySudoku()
    
    // Act
    for row in 0..<9 {
        for column in 0..<9 {
            sudoku[row, column] = cells[row * 9 + column]
        }
    }
    
    // Assert
    for row in 0..<9 {
        for column in 0..<9 {
            #expect(sudoku[row, column] == cells[row * 9 + column])
        }
    }
}

@Test
fileprivate func testArraySudoku_initializer() {
    // Act
    let result = ArraySudoku()
    
    // Assert
    for row in 0..<9 {
        for column in 0..<9 {
            #expect(result[row, column] == 0)
        }
    }
}

@Test
fileprivate func testArraySudoku_initializer_sudoku() {
    // Arrange
    let array = randomArray()
    var sudoku = MockSudoku()
    sudoku.subscriptClosure = { array[$0 * 9 + $1] }
    
    // Act
    let result = ArraySudoku(sudoku)
    
    // Assert
    for row in 0..<9 {
        for column in 0..<9 {
            #expect(result[row, column] == array[row * 9 + column])
        }
    }
}
