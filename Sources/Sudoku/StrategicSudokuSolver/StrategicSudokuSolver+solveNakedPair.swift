//
//  StrategicSudokuSolver+solveNakedPair.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    func solveNakedPair<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator) ?? self.solveBlock(using: &generator)
    }
    
    private func solveBlock<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for blockRow in getBlockIndices(using: &generator) {
            for blockColumn in getBlockIndices(using: &generator) {
                for candidate1 in (1..<9).shuffled(using: &generator) {
                    for candidate2 in ((candidate1 + 1)...9).shuffled(using: &generator) {
                        var indices: Set<Int> = []
                        
                        for rowOffset in 0..<3 {
                            let row = blockRow + rowOffset
                            
                            for columnOffset in 0..<3 {
                                let index = row * 9 + blockColumn + columnOffset
                                
                                if let candidates = self.candidates[index],
                                   candidates.count == 2,
                                   candidates.contains(candidate1),
                                   candidates.contains(candidate2) {
                                    indices.insert(index)
                                }
                            }
                        }
                        
                        guard indices.count == 2 else {
                            continue
                        }
                        
                        let index1 = indices.first!
                        let index2 = indices.last!
                        var locations: Set<SudokuSolverMove.Location> = []
                        let cellCandidates: Set<Int> = [candidate1, candidate2]
                        
                        for rowOffset in 0..<3 {
                            let row = blockRow + rowOffset
                            
                            for columnOffset in 0..<3 {
                                let index = row * 9 + blockColumn + columnOffset
                                
                                guard !(index == index1 || index == index2),
                                      let candidates = self.candidates[index] else {
                                    continue
                                }
                                
                                let removedCandidates = cellCandidates.intersection(candidates)
                                
                                guard !removedCandidates.isEmpty else {
                                    continue
                                }
                                
                                let location = SudokuSolverMove.Location(
                                    index / 9,
                                    index % 9,
                                    removedCandidates
                                )
                                
                                locations.insert(location)
                            }
                        }
                        
                        guard !locations.isEmpty else {
                            continue
                        }
                        
                        return .init(for: .nakedPair, at: locations)
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
                    var rows: Set<Int> = []
                    
                    for row in 0..<9 {
                        if let candidates = self.candidates[row * 9 + column],
                           candidates.count == 2,
                           candidates.contains(candidate1),
                           candidates.contains(candidate2) {
                            rows.insert(row)
                        }
                    }
                    
                    guard rows.count == 2 else {
                        continue
                    }
                    
                    let row1 = rows.first!
                    let row2 = rows.last!
                    var locations: Set<SudokuSolverMove.Location> = []
                    let cellCandidates: Set<Int> = [candidate1, candidate2]
                    
                    for row in 0..<9 {
                        guard !(row == row1 || row == row2),
                              let candidates = self.candidates[row * 9 + column] else {
                            continue
                        }
                        
                        let removedCandidates = cellCandidates.intersection(candidates)
                        
                        guard !removedCandidates.isEmpty else {
                            continue
                        }
                        
                        let location = SudokuSolverMove.Location(
                            row,
                            column,
                            removedCandidates
                        )
                        
                        locations.insert(location)
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .nakedPair, at: locations)
                }
            }
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row in getHouseIndices(using: &generator) {
            for candidate1 in (1..<9).shuffled(using: &generator) {
                for candidate2 in ((candidate1 + 1)...9).shuffled(using: &generator) {
                    var columns: Set<Int> = []
                    
                    for column in 0..<9 {
                        if let candidates = self.candidates[row * 9 + column],
                           candidates.count == 2,
                           candidates.contains(candidate1),
                           candidates.contains(candidate2) {
                            columns.insert(column)
                        }
                    }
                    
                    guard columns.count == 2 else {
                        continue
                    }
                    
                    let column1 = columns.first!
                    let column2 = columns.last!
                    var locations: Set<SudokuSolverMove.Location> = []
                    let cellCandidates: Set<Int> = [candidate1, candidate2]
                    
                    for column in 0..<9 {
                        guard !(column == column1 || column == column2),
                              let candidates = self.candidates[row * 9 + column] else {
                            continue
                        }
                        
                        let removedCandidates = cellCandidates.intersection(candidates)
                        
                        guard !removedCandidates.isEmpty else {
                            continue
                        }
                        
                        let location = SudokuSolverMove.Location(
                            row,
                            column,
                            removedCandidates
                        )
                        
                        locations.insert(location)
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .nakedPair, at: locations)
                }
            }
        }
        
        return nil
    }
}
