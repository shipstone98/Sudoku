//
//  SudokuSolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

public protocol SudokuSolver where Sudoku : SudokuProtocol {
    associatedtype Sudoku
    
    var moves: [SudokuSolverMove] { get }
    var sudoku: Sudoku { get }
    
    init<S>(_ sudoku: S) where S : SudokuProtocol
    
    @discardableResult
    mutating func solve<T>(using generator: inout T) -> Bool where T : RandomNumberGenerator
}
