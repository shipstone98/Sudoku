//
//  StrategicSudokuSolver+solveHiddenPair.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 31/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    func solveHiddenPair<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator) ?? self.solveBlock(using: &generator)
    }
    
    private func solveBlock<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for blockRow in getBlockIndices(using: &generator) {
            for blockColumn in getBlockIndices(using: &generator) {
                for candidate1 in (1..<9).shuffled(using: &generator) {
                    for candidate2 in ((candidate1 + 1)...9).shuffled(using: &generator) {
                        var candidate1Indices: Set<Int> = []
                        var candidate2Indices: Set<Int> = []
                        
                        for rowOffset in 0..<3 {
                            let row = blockRow + rowOffset
                            
                            for columnOffset in 0..<3 {
                                let index = row * 9 + blockColumn + columnOffset
                                
                                if let candidates = self.candidates[index] {
                                    if candidates.contains(candidate1) {
                                        candidate1Indices.insert(index)
                                    }
                                    
                                    if candidates.contains(candidate2) {
                                        candidate2Indices.insert(index)
                                    }
                                }
                            }
                        }
                        
                        guard candidate1Indices.count == 2,
                              candidate1Indices == candidate2Indices else {
                            continue
                        }
                        
                        let candidateSet: Set<Int> = [candidate1, candidate2]
                        
                        let locations = candidate2Indices.compactMap {
                            index -> SudokuSolverMove.Location? in
                            guard let candidates = self.candidates[index],
                                  candidates.count > 2 else {
                                return nil
                            }
                            
                            let removedCandidates = candidates.subtracting(candidateSet)
                            
                            return SudokuSolverMove.Location(
                                index / 9,
                                index % 9,
                                removedCandidates
                            )
                        }
                        
                        guard !locations.isEmpty else {
                            continue
                        }
                        
                        return .init(for: .hiddenPair, at: locations)
                    }
                }
            }
        }
        
        return nil
    }
    
    private func solveColumn<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for column in getHouseIndices(using: &generator) {
            for candidate1 in (1..<9).shuffled(using: &generator) {
                for candidate2 in ((candidate1 + 1)...9).shuffled(using: &generator) {
                    var candidate1Rows: Set<Int> = []
                    var candidate2Rows: Set<Int> = []
                    
                    for row in 0..<9 {
                        if let candidates = self.candidates[row * 9 + column] {
                            if candidates.contains(candidate1) {
                                candidate1Rows.insert(column)
                            }
                            
                            if candidates.contains(candidate2) {
                                candidate2Rows.insert(column)
                            }
                        }
                    }
                    
                    guard candidate1Rows.count == 2,
                          candidate1Rows == candidate2Rows else {
                        continue
                    }
                    
                    let candidateSet: Set<Int> = [candidate1, candidate2]
                    
                    let locations = candidate1Rows.compactMap {
                        row -> SudokuSolverMove.Location? in
                        guard let candidates = self.candidates[row * 9 + column],
                              candidates.count > 2 else {
                            return nil
                        }
                        
                        let removedCandidates = candidates.subtracting(candidateSet)
                        
                        return SudokuSolverMove.Location(
                            row,
                            column,
                            removedCandidates
                        )
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .hiddenPair, at: locations)
                }
            }
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row in getHouseIndices(using: &generator) {
            for candidate1 in (1..<9).shuffled(using: &generator) {
                for candidate2 in ((candidate1 + 1)...9).shuffled(using: &generator) {
                    var candidate1Columns: Set<Int> = []
                    var candidate2Columns: Set<Int> = []
                    
                    for column in 0..<9 {
                        if let candidates = self.candidates[row * 9 + column] {
                            if candidates.contains(candidate1) {
                                candidate1Columns.insert(column)
                            }
                            
                            if candidates.contains(candidate2) {
                                candidate2Columns.insert(column)
                            }
                        }
                    }
                    
                    guard candidate1Columns.count == 2,
                          candidate1Columns == candidate2Columns else {
                        continue
                    }
                    
                    let candidateSet: Set<Int> = [candidate1, candidate2]
                    
                    let locations = candidate1Columns.compactMap {
                        column -> SudokuSolverMove.Location? in
                        guard let candidates = self.candidates[row * 9 + column],
                              candidates.count > 2 else {
                            return nil
                        }
                        
                        let removedCandidates = candidates.subtracting(candidateSet)
                        
                        return SudokuSolverMove.Location(
                            row,
                            column,
                            removedCandidates
                        )
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .hiddenPair, at: locations)
                }
            }
        }
        
        return nil
    }
}
