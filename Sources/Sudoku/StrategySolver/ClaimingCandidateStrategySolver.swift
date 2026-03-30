//
//  ClaimingCandidateStrategySolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Utilities

internal struct ClaimingCandidateStrategySolver : StrategySolver {
    private let solver: StrategicSudokuSolver
    
    init(for solver: StrategicSudokuSolver) {
        self.solver = solver
    }
    
    internal func solve<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator)
    }
    
    private func solveColumn<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for column in getHouseIndices(using: &generator) {
            for candidate in getCandidates(using: &generator) {
                var blockRows: Set<Int> = []
                
                for row in 0..<9 {
                    if let candidates = self.solver.candidates[row * 9 + column],
                       candidates.contains(candidate) {
                        blockRows.insert(row - row % 3)
                    }
                }
                
                guard let blockRow = blockRows.single else {
                    continue
                }
                
                let blockColumn = column - column % 3
                var indices: Set<Int> = []
                
                for columnOffset in 0..<3 {
                    let currentColumn = blockColumn + columnOffset
                    
                    guard column != currentColumn else {
                        continue
                    }
                    
                    for rowOffset in 0..<3 {
                        let index = (blockRow + rowOffset) * 9 + currentColumn
                        
                        if let candidates = self.solver.candidates[index],
                           candidates.contains(candidate) {
                            indices.insert(index)
                        }
                    }
                }
                
                guard !indices.isEmpty else {
                    continue
                }
                
                let removedCandidates: Set<Int> = [candidate]
                
                let locations = indices.map {
                    SudokuSolverMove.Location($0 / 9, $0 % 9, removedCandidates)
                }
                
                return .init(for: .claimingCandidate, at: locations)
            }
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row in getHouseIndices(using: &generator) {
            for candidate in getCandidates(using: &generator) {
                var blockColumns: Set<Int> = []
                
                for column in 0..<9 {
                    if let candidates = self.solver.candidates[row * 9 + column],
                       candidates.contains(candidate) {
                        blockColumns.insert(column - column % 3)
                    }
                }
                
                guard let blockColumn = blockColumns.single else {
                    continue
                }
                
                let blockRow = row - row % 3
                var indices: Set<Int> = []
                
                for rowOffset in 0..<3 {
                    let currentRow = blockRow + rowOffset
                    
                    guard row != currentRow else {
                        continue
                    }
                    
                    for columnOffset in 0..<3 {
                        let index = currentRow * 9 + blockColumn + columnOffset
                        
                        if let candidates = self.solver.candidates[index],
                           candidates.contains(candidate) {
                            indices.insert(index)
                        }
                    }
                }
                
                guard !indices.isEmpty else {
                    continue
                }
                
                let removedCandidates: Set<Int> = [candidate]
                
                let locations = indices.map {
                    SudokuSolverMove.Location($0 / 9, $0 % 9, removedCandidates)
                }
                
                return .init(for: .claimingCandidate, at: locations)
            }
        }
        
        return nil
    }
}
