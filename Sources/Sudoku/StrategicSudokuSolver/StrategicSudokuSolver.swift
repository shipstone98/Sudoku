//
//  StrategicSudokuSolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct StrategicSudokuSolver : Codable, Hashable, Sendable, SudokuSolver {
    internal var candidates: [Int: Set<Int>]
    public internal(set) var moves: [SudokuSolverMove]
    public internal(set) var sudoku: ArraySudoku
    
    public init<S>(_ sudoku: S) where S : SudokuProtocol {
        let sudoku = ArraySudoku(sudoku)
        var candidates: [Int: Set<Int>] = [:]
        
        for row in 0..<9 {
            for column in 0..<9 {
                let index = row * 9 + column
                
                guard sudoku.array[index] == 0 else {
                    continue
                }
                
                candidates[index] = sudoku.candidates(row, column)
            }
        }
        
        self.candidates = candidates
        self.moves = []
        self.sudoku = sudoku
    }
    
    @discardableResult
    public func solve<T>(using generator: inout T) -> Bool where T : RandomNumberGenerator {
        false
    }
}
