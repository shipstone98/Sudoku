//
//  StrategicSudokuSolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct StrategicSudokuSolver : Codable, Hashable, Sendable, SudokuSolver {
    public let moves: [SudokuSolverMove]
    public let sudoku: ArraySudoku
    
    public init<S>(_ sudoku: S) where S : SudokuProtocol {
        self.moves = []
        self.sudoku = .init(sudoku)
    }
    
    @discardableResult
    public func solve<T>(using generator: inout T) -> Bool where T : RandomNumberGenerator {
        false
    }
}
