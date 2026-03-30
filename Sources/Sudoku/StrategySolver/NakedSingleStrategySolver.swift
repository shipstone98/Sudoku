//
//  NakedSingleStrategySolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Utilities

internal struct NakedSingleStrategySolver : StrategySolver {
    private let solver: StrategicSudokuSolver
    
    internal init(for solver: StrategicSudokuSolver) {
        self.solver = solver
    }
    
    internal func solve<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for index in getIndices(using: &generator) {
            guard let candidates = self.solver.candidates[index],
                  let candidate = candidates.single else {
                continue
            }
            
            let location = SudokuSolverMove.Location(
                index / 9,
                index % 9,
                addedValue: candidate
            )
            
            return .init(for: .nakedSingle, at: location)
        }
        
        return nil
    }
}
