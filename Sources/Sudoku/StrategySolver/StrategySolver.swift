//
//  StrategySolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

internal protocol StrategySolver {
    init(for solver: StrategicSudokuSolver)
    
    func solve<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator
}
