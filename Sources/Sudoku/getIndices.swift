//
//  getIndices.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

internal func getIndices<T>(using generator: inout T) -> [Int] where T : RandomNumberGenerator {
    (0..<81).shuffled(using: &generator)
}
