//
//  StrategicSudokuSolver+solveFullHouse.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    func solveFullHouse<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator) ?? self.solveBlock(using: &generator)
    }
    
    private func solveBlock<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for blockRow in getBlockIndices(using: &generator) {
            for blockColumn in getBlockIndices(using: &generator) {
                var indices: Set<Int> = []
                
                for rowOffset in 0..<3 {
                    let row = blockRow + rowOffset
                    
                    for columnOffset in 0..<3 {
                        let index = row * 9 + blockColumn + columnOffset
                        
                        if self.sudoku.array[index] == 0 {
                            indices.insert(index)
                        }
                    }
                }
                
                guard let index = indices.single,
                      let candidates = self.candidates[index],
                      let candidate = candidates.first else {
                    continue
                }
                
                let location = SudokuSolverMove.Location(
                    index / 9,
                    index % 9,
                    addedValue: candidate
                )
                
                return .init(for: .fullHouse, at: location)
            }
        }
        
        return nil
    }
    
    private func solveColumn<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for column in getHouseIndices(using: &generator) {
            var rows: Set<Int> = []
            
            for row in 0..<9 {
                if self.sudoku.array[row * 9 + column] == 0 {
                    rows.insert(row)
                }
            }
            
            guard let row = rows.single,
                  let candidates = self.candidates[row * 9 + column],
                  let candidate = candidates.first else {
                continue
            }
            
            let location = SudokuSolverMove.Location(
                row,
                column,
                addedValue: candidate
            )
            
            return .init(for: .fullHouse, at: location)
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row in getHouseIndices(using: &generator) {
            var columns: Set<Int> = []
            
            for column in 0..<9 {
                if self.sudoku.array[row * 9 + column] == 0 {
                    columns.insert(column)
                }
            }
            
            guard let column = columns.single,
                  let candidates = self.candidates[row * 9 + column],
                  let candidate = candidates.first else {
                continue
            }
            
            let location = SudokuSolverMove.Location(
                row,
                column,
                addedValue: candidate
            )
            
            return .init(for: .fullHouse, at: location)
        }
        
        return nil
    }
}
