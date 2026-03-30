//
//  expectEqual.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing

@testable
import Sudoku

internal func expectEqual<E, A>(_ expected: E, _ actual: A) where E : SudokuProtocol, A : SudokuProtocol {
    for row in 0..<9 {
        for column in 0..<9 {
            #expect(actual[row, column] == expected[row, column])
        }
    }
}

internal func expectEqual<S>(_ expected: String, _ actual: S) where S : SudokuProtocol {
    var index = expected.startIndex
    
    for row in 0..<9 {
        for column in 0..<9 {
            #expect(actual[row, column] == expected[index].wholeNumberValue)
            index = expected.index(after: index)
        }
    }
}
