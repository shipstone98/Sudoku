//
//  HiddenSingleStrategySolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Utilities

internal struct HiddenSingleStrategySolver : StrategySolver {
    private let solver: StrategicSudokuSolver
    
    internal init(for solver: StrategicSudokuSolver) {
        self.solver = solver
    }
    
    internal func solve<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator) ?? self.solveBlock(using: &generator)
    }
    
    private func solveBlock<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for blockRow in getBlockIndices(using: &generator) {
            for blockColumn in getBlockIndices(using: &generator) {
                for candidate in getCandidates(using: &generator) {
                    var indices: Set<Int> = []
                    
                    for rowOffset in 0..<3 {
                        let row = blockRow + rowOffset
                        
                        for columnOffset in 0..<3 {
                            let index = row * 9 + blockColumn + columnOffset
                            
                            if let candidates = self.solver.candidates[index],
                               candidates.contains(candidate) {
                                indices.insert(index)
                            }
                        }
                    }
                    
                    guard let index = indices.single else {
                        continue
                    }
                    
                    let location = SudokuSolverMove.Location(
                        index / 9,
                        index % 9,
                        addedValue: candidate
                    )
                    
                    return .init(for: .hiddenSingle, at: location)
                }
            }
        }
        
        return nil
    }
    
    private func solveColumn<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for column in getHouseIndices(using: &generator) {
            for candidate in getCandidates(using: &generator) {
                var rows: Set<Int> = []
                
                for row in 0..<9 {
                    if let candidates = self.solver.candidates[row * 9 + column],
                       candidates.contains(candidate) {
                        rows.insert(row)
                    }
                }
                
                guard let row = rows.single else {
                    continue
                }
                
                let location = SudokuSolverMove.Location(
                    row,
                    column,
                    addedValue: candidate
                )
                
                return .init(for: .hiddenSingle, at: location)
            }
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row in getHouseIndices(using: &generator) {
            for candidate in getCandidates(using: &generator) {
                var columns: Set<Int> = []
                
                for column in 0..<9 {
                    if let candidates = self.solver.candidates[row * 9 + column],
                       candidates.contains(candidate) {
                        columns.insert(column)
                    }
                }
                
                guard let column = columns.single else {
                    continue
                }
                
                let location = SudokuSolverMove.Location(
                    row,
                    column,
                    addedValue: candidate
                )
                
                return .init(for: .hiddenSingle, at: location)
            }
        }
        
        return nil
    }
}
