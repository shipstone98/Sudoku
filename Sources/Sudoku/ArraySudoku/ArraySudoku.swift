//
//  ArraySudoku.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct ArraySudoku : Codable, Hashable, Sendable, SudokuProtocol {
    internal var array: [Int]
    
    public subscript(row: Int, column: Int) -> Int {
        get {
            self.array[row * 9 + column]
        } set {
            self.array[row * 9 + column] = newValue
        }
    }
    
    public init() {
        self.array = .init(repeating: 0, count: 81)
    }
    
    public init<S>(_ sudoku: S) where S : SudokuProtocol {
        var array: [Int] = []
        
        for row in 0..<9 {
            for column in 0..<9 {
                array.append(sudoku[row, column])
            }
        }
        
        self.array = array
    }
}
