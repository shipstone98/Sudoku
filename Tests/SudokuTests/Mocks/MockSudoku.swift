//
//  MockSudoku.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Sudoku

internal struct MockSudoku : SudokuProtocol {
    internal var subscriptClosure: (Int, Int) -> Int
    
    internal subscript(row: Int, column: Int) -> Int {
        self.subscriptClosure(row, column)
    }
    
    internal init() {
        self.subscriptClosure = { _, _ in 0 }
    }
}
